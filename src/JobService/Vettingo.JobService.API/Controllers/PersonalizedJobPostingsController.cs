using FlashMediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.CreatePersonalizedJobPosting;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.DeletePersonalizedJobPosting;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetAll;

namespace Vettingo.JobService.API.Controllers;

[ApiController]
[Route("api/personalized-job-postings")]
[Authorize]
public class PersonalizedJobPostingsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePersonalizedJobPostingCommandRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Ok();
    }

    [HttpDelete("{personalizedJobPostingId:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid personalizedJobPostingId,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new DeletePersonalizedJobPostingCommandRequest
        {
            PersonalizedJobPostingId = personalizedJobPostingId
        }, cancellationToken);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllPersonalizedJobPostingsQueryRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(request, cancellationToken));
    }
}
