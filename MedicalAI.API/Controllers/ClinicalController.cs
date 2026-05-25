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

    /// <summary>
    /// Nhận form khám sức khỏe, lưu DB, gọi AI, trả kết quả ngay.
    /// Frontend dùng response này để render ResultDashboard lần đầu.
    /// </summary>
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitCheckup([FromBody] CreateCheckupRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "Token không hợp lệ hoặc đã hết hạn." });

        try
        {
            var result = await _clinicalService.SubmitCheckupAsync(userId, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // Ghi log production, trả về message chung tránh lộ thông tin nội bộ
            return StatusCode(500, new { message = "Lỗi hệ thống khi xử lý hồ sơ khám.", detail = ex.Message });
        }
    }

    /// <summary>
    /// Lấy kết quả của một lần khám theo ID.
    /// Frontend gọi endpoint này khi user F5 trang /result/:checkupId.
    /// </summary>
    [HttpGet("{checkupId:long}/result")]
    public async Task<IActionResult> GetCheckupResult(long checkupId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "Token không hợp lệ." });

        var result = await _clinicalService.GetCheckupResultAsync(userId, checkupId);

        if (result == null)
            return NotFound(new { message = $"Không tìm thấy hồ sơ khám #{checkupId}." });

        return Ok(result);
    }
}