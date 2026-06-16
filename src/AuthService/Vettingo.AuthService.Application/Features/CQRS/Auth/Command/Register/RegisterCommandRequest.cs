using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Register
{
    public record RegisterCommandRequest : IRequest
    {
        public string Name { get; init; }
        public string Surname { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
        public string Role { get; init; }

    }
}
