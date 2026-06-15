using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Revoke
{
    public record RevokeCommandRequest : IRequest
    {
        public string Email { get; init; }
        public string RefreshToken { get; init; }
    }
}
