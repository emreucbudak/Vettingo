using FlashMediator;
using System.Text.Json.Serialization;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Register;

public record RegisterCommandRequest : IRequest
{
    public int Code { get; init; }

    [JsonIgnore]
    public string Email { get; init; } = string.Empty;
}
