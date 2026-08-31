using FlashMediator;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.CandidateRegister;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.EmployerRegister;

namespace Vettingo.AuthService.Application.Features.CQRS.Payment.Command.ActivateFreeSubscription;

public sealed class ActivateFreeSubscriptionCommandHandler(IMediator mediator)
    : IRequestHandler<
        ActivateFreeSubscriptionCommandRequest,
        ActivateFreeSubscriptionCommandResponse>
{
    public async Task<ActivateFreeSubscriptionCommandResponse> Handle(
        ActivateFreeSubscriptionCommandRequest request,
        CancellationToken cancellationToken)
    {
        string accountType = request.AccountType.Trim().ToLowerInvariant();
        string billingPeriod = request.BillingPeriod.Trim().ToLowerInvariant();
        string planId = request.PlanId.Trim().ToLowerInvariant();

        if (planId != "basic")
        {
            throw new BadRequestException(
                "Ücretsiz aktivasyon yalnızca Basic plan için kullanılabilir.");
        }

        switch (accountType)
        {
            case "candidate":
                await mediator.Send(
                    new CandidateRegisterCommandRequest
                    {
                        Token = request.RegistrationToken
                    },
                    cancellationToken);
                break;

            case "employer":
                await mediator.Send(
                    new EmployerRegisterCommandRequest
                    {
                        Token = request.RegistrationToken,
                        PlanCode = planId,
                        BillingPeriod = billingPeriod
                    },
                    cancellationToken);
                break;

            default:
                throw new BadRequestException("Geçersiz hesap türü.");
        }

        return new ActivateFreeSubscriptionCommandResponse
        {
            Completed = true
        };
    }
}
