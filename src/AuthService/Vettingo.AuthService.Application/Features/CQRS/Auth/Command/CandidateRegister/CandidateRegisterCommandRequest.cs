using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.CandidateRegister
{
    public sealed record CandidateRegisterCommandRequest : IRequest
    {
        public Guid Token { get; init; }
        public Guid SubscriberId { get; init; }
    }
}
