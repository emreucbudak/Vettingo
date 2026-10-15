using FlashMediator;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.RecordJobPostingPerformance;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetJobPostingPerformance;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyHiringVolume;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyCandidateSource;

namespace Vettingo.AnalyticsService.API.Controllers
{
    [Route("api/analytics")]
    [ApiController]
    public class AnalyticsController(IMediator mediator) : ControllerBase
    {
        [Authorize(Roles = "Company,Human Resources")]
        [HttpGet("companies/{companyId:guid}/hiring-volume")]
        [HttpGet("company/hiring-volume")]
        public async Task<IActionResult> GetCompanyHiringVolume(
            [FromRoute] Guid companyId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirstValue("companyId"), out var currentCompanyId) || currentCompanyId == Guid.Empty)
                return Unauthorized();
            if (companyId != Guid.Empty && companyId != currentCompanyId)
                return Forbid();

            return Ok(await mediator.Send(new GetCompanyHiringVolumeQueryRequest(currentCompanyId), cancellationToken));
        }

        [Authorize(Roles = "Company,Human Resources")]
        [HttpGet("companies/{companyId:guid}/candidate-source")]
        [HttpGet("company/candidate-source")]
        public async Task<IActionResult> GetCompanyCandidateSource(
            [FromRoute] Guid companyId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirstValue("companyId"), out var currentCompanyId) || currentCompanyId == Guid.Empty)
                return Unauthorized();
            if (companyId != Guid.Empty && companyId != currentCompanyId)
                return Forbid();

            return Ok(await mediator.Send(new GetCompanyCandidateSourceQueryRequest(currentCompanyId), cancellationToken));
        }

        [Authorize(Roles = "Company")]
        [HttpPost("job-postings/performance")]
        public async Task<IActionResult> RecordJobPostingPerformance([FromBody] RecordJobPostingPerformanceCommandRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [Authorize(Roles = "Company")]
        [HttpGet("job-postings/{jobPostingId:guid}/performance")]
        public async Task<IActionResult> GetJobPostingPerformance([FromRoute] GetJobPostingPerformanceQueryRequest request)
        {
            return Ok(await mediator.Send(request));
        }

    }
}
