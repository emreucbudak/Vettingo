using FlashMediator;
using Microsoft.AspNetCore.Mvc;
using Vettingo.AuthService.Application.Features.CQRS.Payment.Command.ActivateFreeSubscription;
using Vettingo.AuthService.Application.Features.CQRS.Payment.Command.ConfirmSubscriptionPayment;

namespace Vettingo.AuthService.API.Controllers;

[ApiController]
[Route("api/payments")]
public sealed class PaymentsController(IMediator mediator) : ControllerBase
{
    [HttpPost("subscriptions/activate-free")]
    public async Task<IActionResult> ActivateFreeSubscription(
        [FromBody] ActivateFreeSubscriptionCommandRequest request,
        CancellationToken cancellationToken)
    {
        ActivateFreeSubscriptionCommandResponse response = await mediator.Send(
            request,
            cancellationToken);

        return Ok(response);
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
