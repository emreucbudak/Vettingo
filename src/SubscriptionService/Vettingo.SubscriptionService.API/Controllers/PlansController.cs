using FlashMediator;
using Microsoft.AspNetCore.Mvc;
using Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Command.CreatePlan;
using Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Command.DeletePlan;
using Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Command.UpdatePlan;

namespace Vettingo.SubscriptionService.API.Controllers;

[ApiController]
[Route("api/plans")]
public sealed class PlansController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePlanCommandRequest request,
        CancellationToken cancellationToken)
    {
        int planId = await mediator.Send(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new { id = planId });
    }

    [HttpPut("{planId:int}")]
    public async Task<IActionResult> Update(
        int planId,
        [FromBody] UpdatePlanCommandRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(request with { PlanId = planId }, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{planId:int}")]
    public async Task<IActionResult> Delete(int planId, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new DeletePlanCommandRequest { PlanId = planId },
            cancellationToken);

        return NoContent();
    }
}
