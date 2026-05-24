using MedicalAI.Core.DTOs;
using MedicalAI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAI.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (!result) return BadRequest(new { message = "Email đã tồn tại!" });
        return Ok(new { message = "Đăng ký thành công!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        if (response == null) return Unauthorized(new { message = "Email hoặc mật khẩu không đúng!" });
        return Ok(response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequest request)
    {
        if (ModelState.IsValid)
        {
            var result = await _authService.RefreshTokenAsync(request);
            if (result == null) 
                return BadRequest(new { message = "Token không hợp lệ hoặc đã hết hạn. Vui lòng đăng nhập lại." });
            
            return Ok(result);
        }
        return BadRequest("Dữ liệu gửi lên không hợp lệ.");
    }
}