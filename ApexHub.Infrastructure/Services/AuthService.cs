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
    private readonly JwtService _jwtService;
    private readonly IOtpService _otpService;

    public AuthService(
        ApexHubDbContext context,
        JwtService jwtService,
        IOtpService otpService)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
        _jwtService = jwtService;
        _otpService = otpService;
    }

    public async Task<RegisterResponse?> RegisterAsync(
        RegisterRequest request)
    {
        // Password confirmation
        if (request.Password != request.ConfirmPassword)
        {
            return null;
        }

        // Check username
        var usernameExists = await _context.Users
            .AnyAsync(x => x.Username == request.Username);

        if (usernameExists)
        {
            return null;
        }

        // Check email
        var emailExists = await _context.Users
            .AnyAsync(x => x.Email == request.Email);

        if (emailExists)
        {
            return null;
        }

        // Check mobile number
        var mobileExists = await _context.Users
            .AnyAsync(x => x.MobileNumber == request.MobileNumber);

        if (mobileExists)
        {
            return null;
        }

        // Create user
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

        // Hash password
        user.PasswordHash =
            _passwordHasher.HashPassword(
                user,
                request.Password);

        // Save user
        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        // Generate OTP
        var otpGenerated =
            await _otpService.GenerateOtpAsync(
                new GenerateOtpRequest
                {
                    UserId = user.Id,
                    Purpose = "Registration"
                });

        if (!otpGenerated)
        {
            return null;
        }

        // Return registration information
        return new RegisterResponse
        {
            Message = "Registration successful. OTP sent.",
            UserId = user.Id,
            Username = user.Username,
            MobileNumber = user.MobileNumber
        };
    }

    public async Task<LoginResponse?> LoginAsync(
    LoginRequest request)
    {
        // Find user
        var user = await _context.Users
            .FirstOrDefaultAsync(
                x => x.Username == request.Username);

        // User doesn't exist or is inactive
        if (user == null || !user.IsActive)
        {
            return null;
        }

        // Check phone verification
        if (!user.IsPhoneVerified)
        {
            return null;
        }

        // Verify password
        var result =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

        if (result != PasswordVerificationResult.Success)
        {
            return null;
        }

        // Generate JWT
        var token = _jwtService.GenerateToken(
            user.Id,
            user.Username,
            user.Role);

        return new LoginResponse
        {
            Message = "Login successful.",
            Token = token,
            Username = user.Username,
            Role = user.Role
        };
    }
}