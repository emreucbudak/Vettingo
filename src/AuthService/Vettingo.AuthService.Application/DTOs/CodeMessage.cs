namespace Vettingo.AuthService.Application.DTOs;

public sealed record CodeMessage
{
    public string Email { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
}
