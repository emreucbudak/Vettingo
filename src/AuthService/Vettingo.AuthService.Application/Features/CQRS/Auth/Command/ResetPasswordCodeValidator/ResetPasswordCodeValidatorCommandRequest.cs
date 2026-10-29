using FlashMediator;
using System.Text.Json.Serialization;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ResetPasswordCodeValidator;

public record ResetPasswordCodeValidatorCommandRequest : IRequest<ResetPasswordCodeValidatorCommandResponse>
{
    public string Code { get; init; } = string.Empty;

    [JsonIgnore]
    public string Email { get; init; } = string.Empty;
}
