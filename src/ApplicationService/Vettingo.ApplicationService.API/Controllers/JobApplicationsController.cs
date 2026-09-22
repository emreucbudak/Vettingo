using FlashMediator;
using System.Security.Claims;
using Vettingo.ApplicationService.Application.Repository;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Company")]
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics(
            [FromServices] IHttpClientFactory clients,
            [FromServices] IJobApplicationRepository repository,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirstValue("companyId"), out var companyId) || companyId == Guid.Empty)
                return Unauthorized();

            using var request = new HttpRequestMessage(HttpMethod.Get, "api/job-postings/mine/ids");
            request.Headers.Authorization = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(Request.Headers.Authorization.ToString());
            using var response = await clients.CreateClient("JobService").SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status502BadGateway, "İlan bilgileri alınamadı.");
            var ids = await response.Content.ReadFromJsonAsync<Guid[]>(cancellationToken)
                ?? throw new InvalidOperationException("İlan bilgileri alınamadı.");
            return Ok(await repository.GetStatisticsAsync(ids, cancellationToken));
        }

        [Authorize(Roles = "Company,Candidate")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetJobApplicationsQueryRequest request) =>
            Ok(await mediator.Send(request));

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
