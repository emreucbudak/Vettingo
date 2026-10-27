using FlashMediator;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.DeletePersonalizedJobPosting;

namespace Vettingo.JobService.API.Controllers;

[ApiController]
[Route("api/personalized-job-postings")]
[Authorize]
public class PersonalizedJobPostingsController(IMediator mediator) : ControllerBase
{
    [HttpGet("user")]
    [Authorize(Roles = "Candidate")]
    public IActionResult GetByUser()
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(subject, out var userId) || userId == Guid.Empty)
            return Unauthorized();

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

}
