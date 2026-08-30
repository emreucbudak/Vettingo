using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.EmployerRegister
{
    public record EmployerRegisterCommandRequest : IRequest
    {
        public Guid Token { get; init; }
        public string PlanCode { get; init; } = string.Empty;
        public string BillingPeriod { get; init; } = string.Empty;
    }
}
