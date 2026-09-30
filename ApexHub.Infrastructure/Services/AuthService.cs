using ApexHub.Application.DTOs;
using ApexHub.Application.Interfaces;
using ApexHub.Domain.Entities;
using ApexHub.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApexHub.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApexHubDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthService(ApexHubDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            return false;
        }

        var usernameExists = await _context.Users
            .AnyAsync(x => x.Username == request.Username);

        if (usernameExists)
        {
            return false;
        }

        var emailExists = await _context.Users
            .AnyAsync(x => x.Email == request.Email);

        if (emailExists)
        {
            return false;
        }

        var mobileExists = await _context.Users
            .AnyAsync(x => x.MobileNumber == request.MobileNumber);

        if (mobileExists)
        {
            return false;
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            MobileNumber = request.MobileNumber,
            Role = "User",
            IsActive = true,
            IsPhoneVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            request.Password);

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Username == request.Username);

        if (user == null || !user.IsActive)
        {
            return false;
        }

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        return result == PasswordVerificationResult.Success;
    }
}