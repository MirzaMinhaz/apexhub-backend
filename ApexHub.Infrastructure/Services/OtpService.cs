using ApexHub.Application.DTOs;
using ApexHub.Application.Interfaces;
using ApexHub.Domain.Entities;
using ApexHub.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ApexHub.Infrastructure.Services;

public class OtpService : IOtpService
{
    private readonly ApexHubDbContext _context;
    private readonly PasswordHasher<UserOtp> _passwordHasher;
    private readonly ISmsService _smsService;

    public OtpService(
        ApexHubDbContext context,
        ISmsService smsService)
    {
        _context = context;
        _smsService = smsService;
        _passwordHasher = new PasswordHasher<UserOtp>();
    }

    public async Task<bool> GenerateOtpAsync(
        GenerateOtpRequest request)
    {
        // Find user
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == request.UserId);

        if (user == null)
        {
            return false;
        }

        // Invalidate previous unused OTPs
        var oldOtps = await _context.UserOtps
            .Where(x =>
                x.UserId == request.UserId &&
                x.Purpose == request.Purpose &&
                !x.IsUsed)
            .ToListAsync();

        foreach (var oldOtp in oldOtps)
        {
            oldOtp.IsUsed = true;
        }

        // Generate secure 6-digit OTP
        var otp = RandomNumberGenerator
            .GetInt32(100000, 1000000)
            .ToString();

        // Create OTP record
        var userOtp = new UserOtp
        {
            UserId = request.UserId,
            Purpose = request.Purpose,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            AttemptCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        // Store OTP hash instead of plain OTP
        userOtp.OtpCodeHash =
            _passwordHasher.HashPassword(
                userOtp,
                otp);

        _context.UserOtps.Add(userOtp);

        await _context.SaveChangesAsync();

        // Send OTP through SMS service
        var smsSent = await _smsService.SendOtpAsync(
            user.MobileNumber,
            otp);

        if (!smsSent)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> VerifyOtpAsync(
        VerifyOtpRequest request)
    {
        // Get latest unused OTP
        var otpRecord = await _context.UserOtps
            .Where(x =>
                x.UserId == request.UserId &&
                x.Purpose == request.Purpose &&
                !x.IsUsed)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();

        if (otpRecord == null)
        {
            return false;
        }

        // Check expiry
        if (otpRecord.ExpiresAt < DateTime.UtcNow)
        {
            return false;
        }

        // Maximum 5 attempts
        if (otpRecord.AttemptCount >= 5)
        {
            return false;
        }

        // Increase attempt count
        otpRecord.AttemptCount++;

        // Verify OTP hash
        var result =
            _passwordHasher.VerifyHashedPassword(
                otpRecord,
                otpRecord.OtpCodeHash,
                request.Otp);

        if (result == PasswordVerificationResult.Success)
        {
            // Mark OTP as used
            otpRecord.IsUsed = true;

            // Find user
            var user = await _context.Users
                .FirstOrDefaultAsync(
                    x => x.Id == request.UserId);

            if (user == null)
            {
                return false;
            }

            // Verify user's phone
            user.IsPhoneVerified = true;

            await _context.SaveChangesAsync();

            return true;
        }

        // Save increased attempt count
        await _context.SaveChangesAsync();

        return false;
    }
}