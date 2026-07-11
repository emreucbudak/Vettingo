using FlashMediator;
using Microsoft.AspNetCore.Mvc;
using Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Command.CreateEvaluation;
using Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Command.DeleteEvaluation;
using Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Command.UpdateEvaluation;
using Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Query.GetEvaluationById;
using Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Query.GetEvaluations;

namespace Vettingo.EvaluationService.API.Controllers;

[ApiController]
[Route("api/evaluations")]
public sealed class EvaluationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetEvaluationsQueryRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(request, cancellationToken));
    }

    [HttpGet("{evaluationId:guid}")]
    public async Task<IActionResult> GetById(Guid evaluationId, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(
            new GetEvaluationByIdQueryRequest { EvaluationId = evaluationId },
            cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateEvaluationCommandRequest request,
        CancellationToken cancellationToken)
    {
        Guid evaluationId = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { evaluationId }, new { id = evaluationId });
    }

    [HttpPut("{evaluationId:guid}")]
    public async Task<IActionResult> Update(
        Guid evaluationId,
        [FromBody] UpdateEvaluationCommandRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(request with { EvaluationId = evaluationId }, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{evaluationId:guid}")]
    public async Task<IActionResult> Delete(Guid evaluationId, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new DeleteEvaluationCommandRequest { EvaluationId = evaluationId },
            cancellationToken);

        return NoContent();
    }
}
