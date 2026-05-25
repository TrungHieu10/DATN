using MedicalAI.Core.DTOs;

namespace MedicalAI.Core.Interfaces;

public interface IClinicalService
{
    /// <summary>
    /// Lưu hồ sơ khám + gọi AI + lưu kết quả.
    /// Trả về CheckupResultResponse chứa đủ dữ liệu cho frontend render ngay.
    /// </summary>
    Task<CheckupResultResponse> SubmitCheckupAsync(string userId, CreateCheckupRequest request);

    /// <summary>
    /// Lấy kết quả của một lần khám theo ID.
    /// Dùng khi user F5 trang ResultDashboard.
    /// </summary>
    Task<CheckupResultResponse?> GetCheckupResultAsync(string userId, long checkupId);
}