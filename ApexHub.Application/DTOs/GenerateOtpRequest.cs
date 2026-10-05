namespace ApexHub.Application.DTOs;

public class GenerateOtpRequest
{
    public int UserId { get; set; }

    public string Purpose { get; set; } = "Registration";
}