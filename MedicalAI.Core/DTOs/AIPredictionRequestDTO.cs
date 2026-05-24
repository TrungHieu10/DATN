namespace MedicalAI.Core.DTOs;

public class AIPredictionRequestDTO
{
    public int Age { get; set; }
    public byte Gender { get; set; }

    public decimal? Height_cm { get; set; }
    public decimal? Weight_kg { get; set; }
    public short? SystolicBP { get; set; }
    public short? DiastolicBP { get; set; }
    public decimal? Cholesterol_Total { get; set; }
    public decimal? BloodGlucose { get; set; }
    public byte? SmokingStatus { get; set; }
    public bool? AlcoholConsumption { get; set; }
    public bool? PhysicalActivity { get; set; }
}