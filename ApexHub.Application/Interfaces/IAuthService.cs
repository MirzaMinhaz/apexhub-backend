using ApexHub.Application.DTOs;

namespace ApexHub.Application.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterRequest request);

    Task<bool> LoginAsync(LoginRequest request);
}