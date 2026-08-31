using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.EmployerRegister
{
    public record EmployerRegisterCommandRequest : IRequest
    {
        public Guid Token { get; init; }
        public Guid SubscriberId { get; init; }
    }
}
