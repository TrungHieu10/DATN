using System;
using System.Collections.Generic;

namespace MedicalAI.Core.DTOs
{
    /// <summary>
    /// DTO chứa kết quả dự đoán từ AI model
    /// </summary>
    public class AIPredictionDetailDTO
    {
        public string CheckupId { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Dự đoán chính
        public List<DiseaseRiskDTO> Predictions { get; set; }
        public string RiskLevel { get; set; } // Low, Medium, High, Very High
        public double RiskScore { get; set; } // 0-1

        // SHAP Explainability
        public List<ShapValueDTO> ShapValues { get; set; }

        // RAG-generated advice
        public List<string> Advice { get; set; }
        public List<string> PreventionTips { get; set; }
        public List<string> LifestyleRecommendations { get; set; }

        // Raw metrics for reference
        public HealthMetricsDTO Metrics { get; set; }
    }

    public class DiseaseRiskDTO
    {
        public string Disease { get; set; }
        public string Description { get; set; }
        public double Probability { get; set; } // 0-1
        public string ICD10Code { get; set; }
    }

    public class ShapValueDTO
    {
        public string FeatureName { get; set; }
        public double ShapValue { get; set; }
        public double BaseValue { get; set; }
        public int Index { get; set; }
    }

    public class HealthMetricsDTO
    {
        public int Age { get; set; }
        public double BloodPressureSystolic { get; set; }
        public double BloodPressureDiastolic { get; set; }
        public double HeartRate { get; set; }
        public double Temperature { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public double BMI { get; set; }

        public double BloodGlucose { get; set; }
        public double TotalCholesterol { get; set; }
        public double LDL { get; set; }
        public double HDL { get; set; }
        public double Triglycerides { get; set; }

        public double SleepHours { get; set; }
        public double ExerciseMinutes { get; set; }
        public double StressLevel { get; set; }
        public string SmokingStatus { get; set; }
        public double AlcoholConsumption { get; set; }
        public double FruitsVegetablesServings { get; set; }
    }
}
