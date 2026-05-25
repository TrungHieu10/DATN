using MedicalAI.Core.DTOs;
using MedicalAI.Core.Entities;
using MedicalAI.Core.Interfaces;
using MedicalAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalAI.Infrastructure.Services;

public class ClinicalService : IClinicalService
{
    private readonly ApplicationDbContext _context;
    private readonly IAIPredictionClient _aiClient;

    public ClinicalService(ApplicationDbContext context, IAIPredictionClient aiClient)
    {
        _context = context;
        _aiClient = aiClient;
    }

    public async Task<CheckupResultResponse> SubmitCheckupAsync(string userId, CreateCheckupRequest request)
    {
        // 1. Kiểm tra user tồn tại
        var user = await _context.Users.FindAsync(userId)
            ?? throw new InvalidOperationException("Không tìm thấy người dùng.");

        // 2. Tính tuổi từ ngày sinh
        int age = DateTime.Today.Year - user.DateOfBirth.Year;
        if (user.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

        // 3. Tạo bản ghi HealthCheckup
        var checkup = new HealthCheckup
        {
            UserId    = userId,
            Location  = request.Location,
            Notes     = request.Notes,
            CheckupDate = DateTimeOffset.UtcNow
        };

        // 4. Tạo bản ghi MedicalMetric
        var metric = new MedicalMetric
        {
            HealthCheckup       = checkup,
            Height_cm           = request.Height_cm,
            Weight_kg           = request.Weight_kg,
            SystolicBP          = request.SystolicBP,
            DiastolicBP         = request.DiastolicBP,
            BloodGlucose        = request.BloodGlucose,
            HbA1c               = request.HbA1c,
            Cholesterol_Total   = request.Cholesterol_Total,
            SerumCreatinine     = request.SerumCreatinine,
            BloodUrea           = request.BloodUrea,
            Albumin_Urine       = request.Albumin_Urine,
            Sugar_Urine         = request.Sugar_Urine,
            ALT_SGPT            = request.ALT_SGPT,
            AST_SGOT            = request.AST_SGOT,
            TotalBilirubin      = request.TotalBilirubin,
            DirectBilirubin     = request.DirectBilirubin,
            Hemoglobin          = request.Hemoglobin,
            SmokingStatus       = request.SmokingStatus,
            AlcoholConsumption  = request.AlcoholConsumption,
            PhysicalActivity    = request.PhysicalActivity,
            Hypertension_History  = request.Hypertension_History,
            HeartDisease_History  = request.HeartDisease_History,
            EverMarried         = request.EverMarried,
            WorkType            = request.WorkType,
            ResidenceType       = request.ResidenceType
        };

        _context.HealthCheckups.Add(checkup);
        _context.MedicalMetrics.Add(metric);

        // 5. Lưu tạm để có checkup.Id trước khi gọi AI
        await _context.SaveChangesAsync();

        // 6. Xây dựng DTO đầy đủ gửi sang Python AI
        var aiRequest = new AIPredictionRequestDTO
        {
            Age                  = age,
            Gender               = user.Gender,
            Height_cm            = request.Height_cm,
            Weight_kg            = request.Weight_kg,
            SystolicBP           = request.SystolicBP,
            DiastolicBP          = request.DiastolicBP,
            BloodGlucose         = request.BloodGlucose,
            HbA1c                = request.HbA1c,
            Cholesterol_Total    = request.Cholesterol_Total,
            SerumCreatinine      = request.SerumCreatinine,
            BloodUrea            = request.BloodUrea,
            Albumin_Urine        = request.Albumin_Urine,
            Sugar_Urine          = request.Sugar_Urine,
            ALT_SGPT             = request.ALT_SGPT,
            AST_SGOT             = request.AST_SGOT,
            TotalBilirubin       = request.TotalBilirubin,
            DirectBilirubin      = request.DirectBilirubin,
            Hemoglobin           = request.Hemoglobin,
            SmokingStatus        = request.SmokingStatus,
            AlcoholConsumption   = request.AlcoholConsumption,
            PhysicalActivity     = request.PhysicalActivity,
            Hypertension_History = request.Hypertension_History,
            HeartDisease_History = request.HeartDisease_History,
            EverMarried          = request.EverMarried,
            WorkType             = request.WorkType,
            ResidenceType        = request.ResidenceType
        };

        // 7. Gọi AI Server
        var predictions = await _aiClient.GetPredictionsAsync(aiRequest);

        // 8. Gán CheckupId vào từng kết quả rồi lưu DB
        foreach (var pred in predictions)
        {
            pred.CheckupId = checkup.Id;
            _context.PredictionResults.Add(pred);
        }
        await _context.SaveChangesAsync();

        // 9. Trả về response đầy đủ cho Controller
        return MapToResponse(checkup, predictions);
    }

    public async Task<CheckupResultResponse?> GetCheckupResultAsync(string userId, long checkupId)
    {
        // Bắt buộc filter theo userId để tránh user A xem data user B
        var checkup = await _context.HealthCheckups
            .Include(c => c.PredictionResults)
            .FirstOrDefaultAsync(c => c.Id == checkupId
                                   && c.UserId == userId
                                   && !c.IsDeleted);

        if (checkup == null) return null;

        return MapToResponse(checkup, checkup.PredictionResults.ToList());
    }

    // ── Helper: Entity → DTO ─────────────────────────────────────────
    private static CheckupResultResponse MapToResponse(
        HealthCheckup checkup,
        List<PredictionResult> predictions)
    {
        return new CheckupResultResponse
        {
            CheckupId   = checkup.Id,
            CheckupDate = checkup.CheckupDate,
            Predictions = predictions.Select(p => new PredictionResultDTO
            {
                Id             = p.Id,
                DiseaseType    = p.DiseaseType,
                Probability    = p.Probability,
                RiskLevel      = p.RiskLevel,
                ThresholdUsed  = p.ThresholdUsed,
                ShapValuesJSON = p.ShapValuesJSON,
                AdviceJSON     = p.AdviceJSON,
                ModelVersion   = p.ModelVersion,
                CreatedAt      = p.CreatedAt
            }).ToList()
        };
    }
}