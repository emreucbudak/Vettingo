using FlashMediator;
using Microsoft.AspNetCore.Mvc;
using Vettingo.ApplicationService.Application.Features.CQRS.JobApplication.Command.Create;
using Vettingo.ApplicationService.Application.Features.CQRS.JobApplication.Command.UpdateStatus;
using Vettingo.ApplicationService.Application.Features.CQRS.JobApplication.Query.GetAll;

namespace Vettingo.ApplicationService.API.Controllers
{
    [ApiController]
    [Route("api/job-applications")]
    public class JobApplicationsController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetJobApplicationsQueryRequest request) =>
            Ok(await mediator.Send(request));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateJobApplicationCommandRequest request)
        {
            var response = await mediator.Send(request);
            return StatusCode(StatusCodes.Status201Created, response);
        }

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
