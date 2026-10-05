using ApexHub.Application.DTOs;

namespace ApexHub.Application.Interfaces;

public interface IOtpService
{
    Task<bool> GenerateOtpAsync(
        GenerateOtpRequest request);

    Task<bool> VerifyOtpAsync(
        VerifyOtpRequest request);
}