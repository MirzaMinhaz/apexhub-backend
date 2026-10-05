namespace ApexHub.Application.DTOs;

public class VerifyOtpRequest
{
    public int UserId { get; set; }

    public string Otp { get; set; } = string.Empty;

    public string Purpose { get; set; } = "Registration";
}