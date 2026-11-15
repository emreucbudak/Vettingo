using FlashMediator;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vettingo.EvaluationService.Application.Features.CQRS.Cv.Command.Create;
using Vettingo.EvaluationService.Application.Features.CQRS.Cv.Command.Update;
using Vettingo.EvaluationService.Application.Features.CQRS.Cv.Query.HasCv;

namespace Vettingo.EvaluationService.API.Controllers;

[ApiController]
[Route("api/evaluations")]
[Authorize]
public sealed class EvaluationsController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Candidate")]
    [HttpPost("cv")]
    public async Task<IActionResult> CreateCv(
        [FromBody] CreateCvCommandRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created);
    }

    [Authorize(Roles = "Candidate")]
    [HttpPut("cv")]
    public async Task<IActionResult> UpdateCv(
        [FromBody] UpdateCvCommandRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Candidate")]
    [HttpGet("cv/exists")]
    public async Task<ActionResult<bool>> HasCv(CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(subject, out var userId) || userId == Guid.Empty)
            return Unauthorized();

        return Ok(await mediator.Send(new HasCvQueryRequest(userId), cancellationToken));
    }

    [Authorize(Roles = "Company,Human Resources")]
    [HttpGet("scout")]
    public IActionResult Scout()
    {
        return Ok();
    }
}
