using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.CandidateRegister
{
    public sealed record CandidateRegisterCommandRequest : IRequest
    {
        public Guid Token { get; init; }
        public string PlanCode { get; init; } = string.Empty;
        public string BillingPeriod { get; init; } = string.Empty;
    }
}
