namespace MedicalAI.Core.DTOs;

public class CreateCheckupRequest
{
    public string? Location { get; set; }
    public string? Notes { get; set; }

    // Nhóm Sinh hiệu
    public decimal? Height_cm { get; set; }
    public decimal? Weight_kg { get; set; }
    public short? SystolicBP { get; set; }
    public short? DiastolicBP { get; set; }

    // Nhóm Xét nghiệm
    public decimal? BloodGlucose { get; set; }
    public decimal? HbA1c { get; set; }
    public decimal? Cholesterol_Total { get; set; }
    public decimal? SerumCreatinine { get; set; }
    public decimal? BloodUrea { get; set; }
    public byte? Albumin_Urine { get; set; }
    public byte? Sugar_Urine { get; set; }
    public decimal? ALT_SGPT { get; set; }
    public decimal? AST_SGOT { get; set; }
    public decimal? TotalBilirubin { get; set; }
    public decimal? DirectBilirubin { get; set; }
    public decimal? Hemoglobin { get; set; }

    // Nhóm Lối sống
    public byte? SmokingStatus { get; set; }
    public bool? AlcoholConsumption { get; set; }
    public bool? PhysicalActivity { get; set; }
    public bool? Hypertension_History { get; set; }
    public bool? HeartDisease_History { get; set; }
    public bool? EverMarried { get; set; }
    public byte? WorkType { get; set; }
    public byte? ResidenceType { get; set; }
}