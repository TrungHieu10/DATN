using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAI.Infrastructure.Services
{
    /// <summary>
    /// RAG Engine - Retrieval Augmented Generation
    /// Truy vấn Knowledge Graph để lấy advice và recommendations dựa trên predictions
    /// </summary>
    public interface IRAGEngine
    {
        Task<List<string>> GenerateAdviceAsync(string disease, double riskScore);
        Task<List<string>> GeneratePreventionAsync(string disease);
        Task<List<string>> GenerateLifestyleRecommendationsAsync(List<string> riskFactors);
        Task<string> AugmentAdviceWithContextAsync(string disease, double riskScore);
    }

    public class RAGEngine : IRAGEngine
    {
        private readonly INeo4jService _neo4jService;

        public RAGEngine(INeo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }

        /// <summary>
        /// Sinh lời khuyên y tế dựa trên bệnh và mức độ rủi ro
        /// </summary>
        public async Task<List<string>> GenerateAdviceAsync(string disease, double riskScore)
        {
            var advice = await _neo4jService.GetAdviceByRiskLevelAsync(disease, riskScore);
            
            if (!advice.Any())
            {
                // Fallback advice nếu không có dữ liệu từ KG
                advice = GetDefaultAdviceByRiskLevel(disease, riskScore);
            }

            return advice;
        }

        /// <summary>
        /// Sinh mẹo phòng chống bệnh
        /// </summary>
        public async Task<List<string>> GeneratePreventionAsync(string disease)
        {
            var prevention = await _neo4jService.GetPreventionTipsAsync(disease);
            
            if (!prevention.Any())
            {
                prevention = GetDefaultPrevention(disease);
            }

            return prevention;
        }

        /// <summary>
        /// Sinh khuyến nghị lối sống dựa trên yếu tố nguy hiểm
        /// </summary>
        public async Task<List<string>> GenerateLifestyleRecommendationsAsync(List<string> riskFactors)
        {
            var recommendations = await _neo4jService.GetLifestyleRecommendationsAsync(riskFactors);
            
            if (!recommendations.Any())
            {
                recommendations = GetDefaultLifestyleRecommendations();
            }

            return recommendations;
        }

        /// <summary>
        /// Sinh lời khuyên chi tiết với ngữ cảnh (prompt engineering)
        /// </summary>
        public async Task<string> AugmentAdviceWithContextAsync(string disease, double riskScore)
        {
            var advice = await GenerateAdviceAsync(disease, riskScore);
            var prevention = await GeneratePreventionAsync(disease);
            var riskLevel = GetRiskLevel(riskScore);

            var context = new
            {
                Disease = disease,
                RiskLevel = riskLevel,
                RiskScore = Math.Round(riskScore * 100, 2),
                Advice = advice,
                Prevention = prevention
            };

            // Tạo prompt cho LLM (nếu sử dụng OpenAI hoặc LLM khác)
            var prompt = $@"
Based on the following medical data:
- Disease: {disease}
- Risk Level: {riskLevel}
- Risk Score: {context.RiskScore}%

Medical Advice:
{string.Join("\n", advice.Select((a, i) => $"{i + 1}. {a}"))}

Prevention Tips:
{string.Join("\n", prevention.Select((p, i) => $"{i + 1}. {p}"))}

Please provide a concise, professional medical recommendation in Vietnamese (max 2-3 sentences).
";

            // Đây là placeholder - trong thực tế sẽ call LLM API
            return GenerateFinalRecommendation(disease, riskLevel, advice, prevention);
        }

        private string GetRiskLevel(double riskScore)
        {
            if (riskScore < 0.3) return "Low";
            if (riskScore < 0.6) return "Medium";
            if (riskScore < 0.8) return "High";
            return "Very High";
        }

        private List<string> GetDefaultAdviceByRiskLevel(string disease, double riskScore)
        {
            var riskLevel = GetRiskLevel(riskScore);

            return disease.ToLower() switch
            {
                "diabetes" when riskLevel == "Low" => new List<string>
                {
                    "Maintain your current healthy lifestyle",
                    "Monitor blood glucose levels regularly",
                    "Continue regular check-ups annually",
                    "Keep a balanced diet with portion control"
                },
                "diabetes" when riskLevel == "Medium" => new List<string>
                {
                    "Monitor blood glucose levels every 3 months",
                    "Increase physical activity to 150 minutes/week",
                    "Reduce refined sugar and processed foods",
                    "Consult with a nutritionist for dietary management"
                },
                "diabetes" when riskLevel == "High" => new List<string>
                {
                    "Immediate consultation with an endocrinologist",
                    "Consider medication management",
                    "Monitor glucose daily using a glucose meter",
                    "Attend diabetes management classes"
                },

                "hypertension" when riskLevel == "Low" => new List<string>
                {
                    "Maintain current BP management",
                    "Continue regular aerobic exercise",
                    "Monitor blood pressure monthly",
                    "Reduce sodium intake gradually"
                },
                "hypertension" when riskLevel == "Medium" => new List<string>
                {
                    "Monitor blood pressure weekly",
                    "Reduce sodium to less than 2,300mg/day",
                    "Start or increase regular exercise",
                    "Consider lifestyle modification programs"
                },
                "hypertension" when riskLevel == "High" => new List<string>
                {
                    "Consult hypertension specialist immediately",
                    "Start antihypertensive medications",
                    "Daily blood pressure monitoring",
                    "Strict dietary sodium restriction"
                },

                "heart disease" when riskLevel == "Low" => new List<string>
                {
                    "Maintain cardiovascular fitness",
                    "Keep cholesterol levels in check",
                    "Exercise 150 minutes per week",
                    "Annual cardiac screening"
                },
                "heart disease" when riskLevel == "Medium" => new List<string>
                {
                    "Get lipid panel done quarterly",
                    "Increase cardiovascular exercise gradually",
                    "Reduce saturated fat intake",
                    "Consult cardiologist for prevention plan"
                },
                "heart disease" when riskLevel == "High" => new List<string>
                {
                    "Urgent consultation with cardiologist",
                    "Consider cardiac imaging (ECG, echocardiogram)",
                    "Start cardiac medications if prescribed",
                    "Enroll in cardiac rehabilitation program"
                },

                _ => new List<string>
                {
                    "Consult with your healthcare provider",
                    "Maintain regular check-ups",
                    "Follow a healthy lifestyle",
                    "Keep medical records updated"
                }
            };
        }

        private List<string> GetDefaultPrevention(string disease)
        {
            return disease.ToLower() switch
            {
                "diabetes" => new List<string>
                {
                    "Maintain healthy BMI (18.5-24.9)",
                    "Exercise 30 minutes daily",
                    "Avoid sugary drinks and processed foods",
                    "Monitor weight regularly",
                    "Get 7-8 hours of sleep"
                },
                "hypertension" => new List<string>
                {
                    "Keep BP below 130/80 mmHg",
                    "Limit salt intake to 2.3g/day",
                    "Exercise regularly (30 min/day)",
                    "Manage stress effectively",
                    "Avoid excessive alcohol"
                },
                "heart disease" => new List<string>
                {
                    "Keep cholesterol below 200 mg/dL",
                    "Avoid smoking",
                    "Maintain healthy weight",
                    "Exercise regularly",
                    "Eat heart-healthy diet (Mediterranean)"
                },
                _ => new List<string>
                {
                    "Maintain healthy lifestyle",
                    "Regular health check-ups",
                    "Balanced nutrition",
                    "Regular physical activity",
                    "Stress management"
                }
            };
        }

        private List<string> GetDefaultLifestyleRecommendations()
        {
            return new List<string>
            {
                "Start with 30 minutes of moderate exercise, 5 days a week (walking, swimming, cycling)",
                "Include more vegetables, whole grains, and lean proteins in your diet",
                "Reduce sodium intake by avoiding processed foods and cooking at home",
                "Practice yoga, meditation, or deep breathing for 10-15 minutes daily",
                "Maintain consistent sleep schedule (10 PM - 6 AM) for 7-8 hours",
                "Stay hydrated by drinking 8-10 glasses of water daily",
                "Join a health support group or find an accountability partner",
                "Track daily nutrition and activity using health apps like MyFitnessPal or Apple Health",
                "Limit caffeine intake and avoid smoking",
                "Schedule regular health check-ups every 6 months"
            };
        }

        private string GenerateFinalRecommendation(string disease, string riskLevel, List<string> advice, List<string> prevention)
        {
            var topAdvice = advice.FirstOrDefault() ?? "Consult with your healthcare provider";
            
            return riskLevel switch
            {
                "Low" => $"Your risk for {disease} is low. {topAdvice} to maintain your health.",
                "Medium" => $"You have moderate risk for {disease}. {topAdvice} and follow the preventive measures above.",
                "High" => $"You have elevated risk for {disease}. {topAdvice} urgently and implement lifestyle changes immediately.",
                "Very High" => $"You have very high risk for {disease}. Seek immediate medical consultation and strict adherence to treatment.",
                _ => "Please consult with a healthcare professional for personalized recommendations."
            };
        }
    }
}
