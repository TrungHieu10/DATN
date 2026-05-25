using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicalAI.Core.DTOs;
using MedicalAI.Core.Entities;
using MedicalAI.Core.Interfaces;
using MedicalAI.Infrastructure.Data;

namespace MedicalAI.Infrastructure.Services
{
    /// <summary>
    /// Clinical Service - Orchestration layer
    /// Tích hợp AI predictions + RAG Engine + Database
    /// </summary>
    public interface IClinicalServiceWithRAG
    {
        Task<AIPredictionDetailDTO> GetPredictionWithAdviceAsync(string checkupId);
        Task<AIPredictionDetailDTO> SubmitCheckupWithAdviceAsync(AIPredictionRequestDTO request, int userId);
    }

    public class ClinicalServiceWithRAG : IClinicalServiceWithRAG
    {
        private readonly IAIPredictionClient _aiClient;
        private readonly IRAGEngine _ragEngine;
        private readonly ApplicationDbContext _dbContext;

        public ClinicalServiceWithRAG(
            IAIPredictionClient aiClient,
            IRAGEngine ragEngine,
            ApplicationDbContext dbContext)
        {
            _aiClient = aiClient;
            _ragEngine = ragEngine;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Lấy kết quả dự đoán + lời khuyên từ Knowledge Graph
        /// </summary>
        public async Task<AIPredictionDetailDTO> GetPredictionWithAdviceAsync(string checkupId)
        {
            try
            {
                // Tìm checkup trong DB
                var checkup = _dbContext.HealthCheckups.FirstOrDefault(c => c.Id == checkupId);
                if (checkup == null)
                    throw new Exception("Checkup not found");

                // Tìm predictions liên quan
                var predictions = _dbContext.PredictionResults
                    .Where(p => p.HealthCheckupId == checkupId)
                    .ToList();

                if (!predictions.Any())
                    throw new Exception("No predictions found for this checkup");

                // Ánh xạ sang DTO
                var dto = MapToDetailDTO(checkup, predictions);

                // Lấy advice từ RAG Engine dựa trên diseases
                var topDisease = predictions.OrderByDescending(p => p.Probability).FirstOrDefault();
                if (topDisease != null)
                {
                    dto.Advice = await _ragEngine.GenerateAdviceAsync(
                        topDisease.DiseaseType,
                        topDisease.Probability);

                    dto.PreventionTips = await _ragEngine.GeneratePreventionAsync(
                        topDisease.DiseaseType);

                    // Lifestyle recommendations dựa trên risk factors
                    var riskFactors = ExtractRiskFactors(checkup);
                    dto.LifestyleRecommendations = await _ragEngine.GenerateLifestyleRecommendationsAsync(
                        riskFactors);

                    // Set risk level
                    dto.RiskLevel = GetRiskLevel(topDisease.Probability);
                    dto.RiskScore = topDisease.Probability;
                }

                return dto;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching prediction with advice: {ex.Message}");
            }
        }

        /// <summary>
        /// Submit checkup mới + nhận dự đoán + sinh advice
        /// </summary>
        public async Task<AIPredictionDetailDTO> SubmitCheckupWithAdviceAsync(
            AIPredictionRequestDTO request, 
            int userId)
        {
            try
            {
                // 1. Gửi request tới AI model (Python)
                var aiPredictions = await _aiClient.GetPredictionsAsync(request);

                // 2. Tạo Checkup entity
                var checkup = new HealthCheckup
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    Metrics = SerializeMetrics(request)
                };

                // 3. Lưu Checkup vào DB
                _dbContext.HealthCheckups.Add(checkup);

                // 4. Lưu Predictions vào DB
                foreach (var pred in aiPredictions)
                {
                    _dbContext.PredictionResults.Add(new PredictionResult
                    {
                        Id = Guid.NewGuid().ToString(),
                        HealthCheckupId = checkup.Id,
                        DiseaseType = pred.DiseaseType,
                        Probability = pred.Probability,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _dbContext.SaveChangesAsync();

                // 5. Sinh advice từ RAG
                var topDisease = aiPredictions.OrderByDescending(p => p.Probability).FirstOrDefault();
                var advice = new List<string>();
                var prevention = new List<string>();
                var lifestyle = new List<string>();

                if (topDisease != null)
                {
                    advice = await _ragEngine.GenerateAdviceAsync(
                        topDisease.DiseaseType,
                        topDisease.Probability);

                    prevention = await _ragEngine.GeneratePreventionAsync(
                        topDisease.DiseaseType);

                    var riskFactors = ExtractRiskFactors(request);
                    lifestyle = await _ragEngine.GenerateLifestyleRecommendationsAsync(riskFactors);
                }

                // 6. Return DTO
                return new AIPredictionDetailDTO
                {
                    CheckupId = checkup.Id,
                    CreatedAt = checkup.CreatedAt,
                    Predictions = aiPredictions
                        .Select(p => new DiseaseRiskDTO
                        {
                            Disease = p.DiseaseType,
                            Probability = p.Probability
                        })
                        .ToList(),
                    RiskLevel = topDisease != null ? GetRiskLevel(topDisease.Probability) : "Unknown",
                    RiskScore = topDisease?.Probability ?? 0,
                    Advice = advice,
                    PreventionTips = prevention,
                    LifestyleRecommendations = lifestyle,
                    Metrics = MapMetrics(request)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error submitting checkup: {ex.Message}");
            }
        }

        private AIPredictionDetailDTO MapToDetailDTO(HealthCheckup checkup, List<PredictionResult> predictions)
        {
            var dto = new AIPredictionDetailDTO
            {
                CheckupId = checkup.Id,
                CreatedAt = checkup.CreatedAt,
                Predictions = predictions
                    .Select(p => new DiseaseRiskDTO
                    {
                        Disease = p.DiseaseType,
                        Probability = p.Probability
                    })
                    .ToList(),
                Advice = new List<string>(),
                PreventionTips = new List<string>(),
                LifestyleRecommendations = new List<string>()
            };

            return dto;
        }

        private List<string> ExtractRiskFactors(HealthCheckup checkup)
        {
            var riskFactors = new List<string>();

            // Parse metrics and identify risk factors
            if (!string.IsNullOrEmpty(checkup.Metrics))
            {
                // Implement logic to extract risk factors from JSON metrics
                if (checkup.Metrics.Contains("\"bloodGlucose\":") && 
                    checkup.Metrics.Contains("100")) // Simplified check
                {
                    riskFactors.Add("High Blood Glucose");
                }

                if (checkup.Metrics.Contains("\"systolicBP\":") && 
                    checkup.Metrics.Contains("140"))
                {
                    riskFactors.Add("High Blood Pressure");
                }

                if (checkup.Metrics.Contains("\"bmi\":") && 
                    checkup.Metrics.Contains("30"))
                {
                    riskFactors.Add("Obesity");
                }

                if (checkup.Metrics.Contains("\"smokingStatus\":\"Current\""))
                {
                    riskFactors.Add("Smoking");
                }

                if (checkup.Metrics.Contains("\"exerciseMinutes\":\"0\"") ||
                    checkup.Metrics.Contains("\"exerciseMinutes\":\"30\""))
                {
                    riskFactors.Add("Sedentary Lifestyle");
                }

                if (checkup.Metrics.Contains("\"stressLevel\":") && 
                    checkup.Metrics.Contains("8"))
                {
                    riskFactors.Add("High Stress");
                }
            }

            return riskFactors.Any() ? riskFactors : new List<string> { "General Health" };
        }

        private List<string> ExtractRiskFactors(AIPredictionRequestDTO request)
        {
            var riskFactors = new List<string>();

            if (request.bloodGlucose > 126) riskFactors.Add("High Blood Glucose");
            if (request.systolicBP > 140 || request.diastolicBP > 90) riskFactors.Add("High Blood Pressure");
            if (request.totalCholesterol > 240) riskFactors.Add("High Cholesterol");
            if (request.weight > 0 && request.height > 0)
            {
                var bmi = request.weight / ((request.height / 100) * (request.height / 100));
                if (bmi >= 30) riskFactors.Add("Obesity");
            }

            return riskFactors.Any() ? riskFactors : new List<string> { "General Health" };
        }

        private string GetRiskLevel(double riskScore)
        {
            if (riskScore < 0.3) return "Low";
            if (riskScore < 0.6) return "Medium";
            if (riskScore < 0.8) return "High";
            return "Very High";
        }

        private string SerializeMetrics(AIPredictionRequestDTO request)
        {
            // Implement JSON serialization of metrics
            return System.Text.Json.JsonSerializer.Serialize(request);
        }

        private HealthMetricsDTO MapMetrics(AIPredictionRequestDTO request)
        {
            return new HealthMetricsDTO
            {
                Age = request.age,
                BloodPressureSystolic = request.systolicBP,
                BloodPressureDiastolic = request.diastolicBP,
                HeartRate = request.heartRate,
                Temperature = request.temperature,
                Weight = request.weight,
                Height = request.height,
                BloodGlucose = request.bloodGlucose,
                TotalCholesterol = request.totalCholesterol,
                LDL = request.ldl,
                HDL = request.hdl,
                Triglycerides = request.triglycerides
            };
        }
    }
}
