using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAI.Core.Entities;

[Table("MedicalMetrics", Schema = "Clinic")]
public class MedicalMetric
{
    [Key]
    public long Id { get; set; }

    public long CheckupId { get; set; }

    // Nhóm Sinh hiệu
    [Column(TypeName = "decimal(5,2)")] public decimal? Height_cm { get; set; }
    [Column(TypeName = "decimal(5,2)")] public decimal? Weight_kg { get; set; }
    public short? SystolicBP { get; set; }
    public short? DiastolicBP { get; set; }

    // Nhóm Xét nghiệm (Đủ cho 5 mô hình AI)
    [Column(TypeName = "decimal(10,2)")] public decimal? BloodGlucose { get; set; }
    [Column(TypeName = "decimal(4,2)")] public decimal? HbA1c { get; set; }
    [Column(TypeName = "decimal(10,2)")] public decimal? Cholesterol_Total { get; set; }
    [Column(TypeName = "decimal(5,2)")] public decimal? SerumCreatinine { get; set; }
    [Column(TypeName = "decimal(10,2)")] public decimal? BloodUrea { get; set; }
    public byte? Albumin_Urine { get; set; }
    public byte? Sugar_Urine { get; set; }
    [Column(TypeName = "decimal(10,2)")] public decimal? ALT_SGPT { get; set; }
    [Column(TypeName = "decimal(10,2)")] public decimal? AST_SGOT { get; set; }
    [Column(TypeName = "decimal(10,2)")] public decimal? TotalBilirubin { get; set; }
    [Column(TypeName = "decimal(10,2)")] public decimal? DirectBilirubin { get; set; }
    [Column(TypeName = "decimal(4,2)")] public decimal? Hemoglobin { get; set; }

    // Nhóm Lối sống / Nhân khẩu
    public byte? SmokingStatus { get; set; }
    public bool? AlcoholConsumption { get; set; }
    public bool? PhysicalActivity { get; set; }
    public bool? Hypertension_History { get; set; }
    public bool? HeartDisease_History { get; set; }
    public bool? EverMarried { get; set; }
    public byte? WorkType { get; set; }
    public byte? ResidenceType { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [ForeignKey("CheckupId")]
    public virtual HealthCheckup HealthCheckup { get; set; } = null!;
}