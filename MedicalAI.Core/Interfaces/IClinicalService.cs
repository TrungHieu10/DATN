using MedicalAI.Core.DTOs;

namespace MedicalAI.Core.Interfaces;

public interface IClinicalService
{
    // Hàm này sẽ lưu hồ sơ và trả về ID của phiên khám bệnh
    Task<long> SubmitCheckupAsync(string userId, CreateCheckupRequest request);
}