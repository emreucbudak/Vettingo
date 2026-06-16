using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Login
{
    public record LoginCommandRequest : IRequest<LoginCommandResponse>
    {
        public string Email { get; init; }
        public string Password { get; init; }
    }
}
