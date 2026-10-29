using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ResetPasswordTemp;

public record ResetPasswordTempCommandRequest : IRequest
{
    public string Email { get; init; } = string.Empty;
}
