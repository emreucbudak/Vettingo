using FlashMediator;
using System.Text.Json.Serialization;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ResetPassword;

public record ResetPasswordCommandRequest : IRequest
{
    public string NewPassword { get; init; } = string.Empty;

    [JsonIgnore]
    public string Email { get; init; } = string.Empty;
}
