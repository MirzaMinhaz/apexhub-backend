using ApexHub.Application.DTOs;
using ApexHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApexHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OtpController : ControllerBase
{
    private readonly IOtpService _otpService;

    public OtpController(IOtpService otpService)
    {
        _otpService = otpService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate(
        GenerateOtpRequest request)
    {
        var result = await _otpService.GenerateOtpAsync(request);

        if (!result)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        return Ok(new
        {
            message = "OTP generated successfully."
        });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify(
        VerifyOtpRequest request)
    {
        var result = await _otpService.VerifyOtpAsync(request);

        if (!result)
        {
            return BadRequest(new
            {
                message = "Invalid or expired OTP."
            });
        }

        return Ok(new
        {
            message = "OTP verified successfully."
        });
    }
}