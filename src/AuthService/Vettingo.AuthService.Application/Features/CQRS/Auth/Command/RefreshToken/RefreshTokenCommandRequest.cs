using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.RefreshToken
{
    public record RefreshTokenCommandRequest : IRequest<RefreshTokenCommandResponse>
    {
        public string AccessToken { get; init; }
    }
}
