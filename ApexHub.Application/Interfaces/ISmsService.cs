namespace ApexHub.Application.Interfaces;

public interface ISmsService
{
    Task<bool> SendOtpAsync(
        string mobileNumber,
        string otp);
}