using ApexHub.Application.DTOs;

namespace ApexHub.Application.Interfaces;

public interface IAuthService
{
    Task<RegisterResponse?> RegisterAsync(
        RegisterRequest request);

    Task<LoginResponse?> LoginAsync(
        LoginRequest request);
}