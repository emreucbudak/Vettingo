namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TempRegister;

public sealed record TempRegisterCommandResponse
{
    public string TempToken { get; init; } = string.Empty;
}
