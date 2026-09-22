using FlashMediator;
using System.Security.Claims;
using Vettingo.JobService.Application.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.Create;
using Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.UpdateStatus;
using Vettingo.JobService.Application.Features.CQRS.JobApplication.Query.GetAll;

namespace Vettingo.JobService.API.Controllers
{
    [ApiController]
    [Route("api/job-applications")]
    public class JobApplicationsController(IMediator mediator) : ControllerBase
    {
        [Authorize(Roles = "Candidate")]
        [HttpGet("my/statistics")]
        public async Task<IActionResult> GetCandidateStatistics(
            [FromServices] IJobApplicationRepository repository, CancellationToken cancellationToken)
        {
            var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!Guid.TryParse(subject, out var candidateId) || candidateId == Guid.Empty)
                return Unauthorized();
            return Ok(await repository.GetCandidateStatisticsAsync(candidateId, cancellationToken));
        }

        [Authorize(Roles = "Company")]
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics(
            [FromServices] IJobApplicationRepository repository,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirstValue("companyId"), out var companyId) || companyId == Guid.Empty)
                return Unauthorized();

            return Ok(await repository.GetStatisticsAsync(companyId, cancellationToken));
        }

        [Authorize(Roles = "Company,Candidate")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetJobApplicationsQueryRequest request)
        {
            if (User.IsInRole("Candidate"))
            {
                var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
                if (!Guid.TryParse(subject, out var candidateId) || candidateId == Guid.Empty)
                    return Unauthorized();

                request = request with { CandidateId = candidateId };
            }

            return Ok(await mediator.Send(request));
        }

        [Authorize(Roles = "Candidate")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateJobApplicationCommandRequest request)
        {
            var response = await mediator.Send(request);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [Authorize(Roles = "Company")]
        [HttpPut("{applicationId:guid}/status")]
        public async Task<IActionResult> UpdateStatus(
            [FromRoute] Guid applicationId,
            [FromBody] UpdateJobApplicationStatusCommandRequest request)
        {
            await mediator.Send(request with { ApplicationId = applicationId });
            return NoContent();
        }
    }
}
