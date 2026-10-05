using ApexHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ApexHub.Infrastructure.Data;

public class ApexHubDbContext : DbContext
{
    public ApexHubDbContext(DbContextOptions<ApexHubDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserOtp> UserOtps => Set<UserOtp>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Username)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.MobileNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(x => x.Role)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.Property(x => x.IsPhoneVerified)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.HasIndex(x => x.Username)
                .IsUnique();

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.HasIndex(x => x.MobileNumber)
                .IsUnique();
        });
        modelBuilder.Entity<UserOtp>(entity =>
        {
            entity.ToTable("UserOtps");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.OtpCodeHash)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(x => x.Purpose)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.ExpiresAt)
                .IsRequired();

            entity.Property(x => x.IsUsed)
                .IsRequired();

            entity.Property(x => x.AttemptCount)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}