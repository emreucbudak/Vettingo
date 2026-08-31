using FlashMediator;
using Microsoft.AspNetCore.Mvc;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.CandidateRegister;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.EmployerRegister;
using Vettingo.AuthService.Application.Features.CQRS.Payment.Command.ConfirmSubscriptionPayment;

namespace Vettingo.AuthService.API.Controllers;

[ApiController]
[Route("api/payments")]
public sealed class PaymentsController(IMediator mediator) : ControllerBase
{
    [HttpPost("subscriptions/activate-free")]
    public async Task<IActionResult> ActivateFreeSubscription(
        [FromBody] FreeSubscriptionActivationRequest request,
        CancellationToken cancellationToken)
    {
        string accountType = request.AccountType.Trim().ToLowerInvariant();
        string billingPeriod = request.BillingPeriod.Trim().ToLowerInvariant();
        string planId = request.PlanId.Trim().ToLowerInvariant();

        if (request.RegistrationToken == Guid.Empty ||
            planId != "basic" ||
            billingPeriod is not ("monthly" or "annual") ||
            accountType is not ("candidate" or "employer"))
        {
            return BadRequest(new
            {
                message = "Ücretsiz abonelik aktivasyon bilgileri geçersiz."
            });
        }

        if (accountType == "candidate")
        {
            await mediator.Send(
                new CandidateRegisterCommandRequest
                {
                    Token = request.RegistrationToken
                },
                cancellationToken);
        }
        else
        {
            await mediator.Send(
                new EmployerRegisterCommandRequest
                {
                    Token = request.RegistrationToken,
                    PlanCode = planId,
                    BillingPeriod = billingPeriod
                },
                cancellationToken);
        }

        return Ok(new { completed = true });
    }

    [HttpPost("subscriptions/confirm")]
    public async Task<IActionResult> ConfirmSubscription(
        [FromBody] ConfirmSubscriptionPaymentCommandRequest request,
        CancellationToken cancellationToken)
    {
        ConfirmSubscriptionPaymentCommandResponse response = await mediator.Send(
            request,
            cancellationToken);

        return Ok(response);
    }
}

public sealed record FreeSubscriptionActivationRequest
{
    public string AccountType { get; init; } = string.Empty;
    public string BillingPeriod { get; init; } = string.Empty;
    public string PlanId { get; init; } = string.Empty;
    public Guid RegistrationToken { get; init; }
}
