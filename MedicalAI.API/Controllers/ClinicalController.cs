using System.Security.Claims;
using MedicalAI.Core.DTOs;
using MedicalAI.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAI.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] 
public class ClinicalController : ControllerBase
{
    private readonly IClinicalService _clinicalService;

    public ClinicalController(IClinicalService clinicalService)
    {
        _clinicalService = clinicalService;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitCheckup([FromBody] CreateCheckupRequest request)
    {
        // Trích xuất an toàn UserId từ Token (Không cho phép người dùng tự gửi ID lên để tránh giả mạo)
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Token không hợp lệ hoặc đã hết hạn.");

        try
        {
            var checkupId = await _clinicalService.SubmitCheckupAsync(userId, request);
            
            // Tạm thời trả về OK. 
            // Ở bước sau, chúng ta sẽ gọi Python AI tại đây để lấy dự đoán trước khi trả về!
            return Ok(new 
            { 
                message = "Đã lưu hồ sơ thành công, chuẩn bị gửi cho AI...",
                checkupId = checkupId
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
        }
    }
}