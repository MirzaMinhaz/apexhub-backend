namespace ApexHub.Application.DTOs;

public class RegisterResponse
{
    public string Message { get; set; } = string.Empty;

    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;
}