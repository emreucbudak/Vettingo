using FlashMediator;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.RecordCandidateCvAnalysis;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.RecordCandidateRecommendation;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.RecordJobPostingPerformance;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCandidateCvAnalysis;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyRecommendationAnalytics;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetJobPostingPerformance;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyHiringVolume;

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

        [Authorize(Roles = "Company")]
        [HttpPost("recommendations")]
        public async Task<IActionResult> RecordCandidateRecommendation([FromBody] RecordCandidateRecommendationCommandRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [Authorize(Roles = "Company")]
        [HttpGet("companies/{companyId:guid}/recommendations")]
        public async Task<IActionResult> GetCompanyRecommendationAnalytics([FromRoute] GetCompanyRecommendationAnalyticsQueryRequest request)
        {
            return Ok(await mediator.Send(request));
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

        [Authorize(Roles = "Candidate")]
        [HttpPost("candidates/cv-analysis")]
        public async Task<IActionResult> RecordCandidateCvAnalysis([FromBody] RecordCandidateCvAnalysisCommandRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [Authorize(Roles = "Candidate")]
        [HttpGet("candidates/{candidateId:guid}/cv-analysis")]
        public async Task<IActionResult> GetCandidateCvAnalysis(
            [FromRoute] Guid candidateId,
            [FromQuery] GetCandidateCvAnalysisQueryRequest request)
        {
            return Ok(await mediator.Send(new GetCandidateCvAnalysisQueryRequest(candidateId,latestOnly:request.LatestOnly)));
        }
    }
}
