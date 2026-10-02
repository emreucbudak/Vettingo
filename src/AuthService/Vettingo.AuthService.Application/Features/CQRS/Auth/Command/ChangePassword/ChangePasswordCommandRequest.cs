using System.Text.Json.Serialization;
using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ChangePassword;

public record ChangePasswordCommandRequest : IRequest
{
    [JsonIgnore]
    public string Email { get; init; } = string.Empty;
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}
