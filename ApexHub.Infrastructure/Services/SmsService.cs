using ApexHub.Application.Interfaces;

namespace ApexHub.Infrastructure.Services;

public class SmsService : ISmsService
{
    public Task<bool> SendOtpAsync(
        string mobileNumber,
        string otp)
    {
        Console.WriteLine(
            $"[DEV SMS] To: {mobileNumber}, OTP: {otp}");

        return Task.FromResult(true);
    }
}