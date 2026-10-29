namespace Vettingo.AuthService.Application.DTOs;

public sealed record TempRegisterData
{
    public string Email { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}
