namespace ApexHub.Domain.Entities;

public class UserOtp
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string OtpCodeHash { get; set; } = string.Empty;

    public string Purpose { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; } = false;

    public int AttemptCount { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}