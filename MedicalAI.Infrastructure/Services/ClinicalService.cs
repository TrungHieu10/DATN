using MedicalAI.Core.DTOs;
using MedicalAI.Core.Entities;
using MedicalAI.Core.Interfaces;
using MedicalAI.Infrastructure.Data;

namespace MedicalAI.Infrastructure.Services;

public class ClinicalService : IClinicalService
{
    private readonly ApplicationDbContext _context;
    private readonly IAIPredictionClient _aiClient; // Thêm AI Client

    // Inject thêm AI Client vào constructor
    public ClinicalService(ApplicationDbContext context, IAIPredictionClient aiClient)
    {
        _context = context;
        _aiClient = aiClient;
    }

    public async Task<long> SubmitCheckupAsync(string userId, CreateCheckupRequest request)
    {
        
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new Exception("Không tìm thấy User");

        // Tính tuổi
        int age = DateTime.Today.Year - user.DateOfBirth.Year;
        if (user.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;
        var checkup = new HealthCheckup
        {
            UserId = userId, Location = request.Location,
            Notes = request.Notes, CheckupDate = DateTimeOffset.UtcNow
        };

        // 2. Tạo Chỉ số Y tế
        var metric = new MedicalMetric
        {
            HealthCheckup = checkup,
            Height_cm = request.Height_cm, Weight_kg = request.Weight_kg,
            SystolicBP = request.SystolicBP, DiastolicBP = request.DiastolicBP,
            BloodGlucose = request.BloodGlucose, HbA1c = request.HbA1c,
            Cholesterol_Total = request.Cholesterol_Total, SerumCreatinine = request.SerumCreatinine,
            BloodUrea = request.BloodUrea, Albumin_Urine = request.Albumin_Urine,
            Sugar_Urine = request.Sugar_Urine, ALT_SGPT = request.ALT_SGPT,
            AST_SGOT = request.AST_SGOT, TotalBilirubin = request.TotalBilirubin,
            DirectBilirubin = request.DirectBilirubin, Hemoglobin = request.Hemoglobin,
            SmokingStatus = request.SmokingStatus, AlcoholConsumption = request.AlcoholConsumption,
            PhysicalActivity = request.PhysicalActivity, Hypertension_History = request.Hypertension_History,
            HeartDisease_History = request.HeartDisease_History, EverMarried = request.EverMarried,
            WorkType = request.WorkType, ResidenceType = request.ResidenceType
        };

        _context.HealthCheckups.Add(checkup);
        _context.MedicalMetrics.Add(metric);

        // Lưu tạm vào DB để lấy Checkup.Id
        await _context.SaveChangesAsync();

        // 3. GỌI AI SERVER ĐỂ CHẨN ĐOÁN
        var aiRequest = new AIPredictionRequestDTO
        {
            Age = age,
            Gender = user.Gender,
            Height_cm = request.Height_cm,
            Weight_kg = request.Weight_kg,
            SystolicBP = request.SystolicBP,
            DiastolicBP = request.DiastolicBP,
            Cholesterol_Total = request.Cholesterol_Total,
            BloodGlucose = request.BloodGlucose,
            SmokingStatus = request.SmokingStatus,
            AlcoholConsumption = request.AlcoholConsumption,
            PhysicalActivity = request.PhysicalActivity
        };

        var predictions = await _aiClient.GetPredictionsAsync(aiRequest);

        // 4. Lưu kết quả AI (Giữ nguyên)
        foreach (var pred in predictions)
        {
            pred.CheckupId = checkup.Id;
            _context.PredictionResults.Add(pred);
        }
        await _context.SaveChangesAsync();
        
        return checkup.Id;
    }
}