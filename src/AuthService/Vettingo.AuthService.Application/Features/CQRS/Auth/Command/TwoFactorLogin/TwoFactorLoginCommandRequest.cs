using FlashMediator;
using System.Text.Json.Serialization;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TwoFactorLogin;

public record TwoFactorLoginCommandRequest : IRequest<TwoFactorLoginCommandResponse>
{
    public int Code { get; init; }

    [JsonIgnore]
    public string Email { get; init; } = string.Empty;
}
