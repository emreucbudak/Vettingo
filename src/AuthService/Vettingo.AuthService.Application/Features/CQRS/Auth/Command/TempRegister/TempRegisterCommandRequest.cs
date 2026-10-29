using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TempRegister;

public record TempRegisterCommandRequest : IRequest<TempRegisterCommandResponse>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string? CompanyName { get; init; }
}
