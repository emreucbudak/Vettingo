using FlashMediator;
using Microsoft.AspNetCore.Mvc;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.RecordCandidateCvAnalysis;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.RecordCandidateRecommendation;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.RecordJobPostingPerformance;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCandidateCvAnalysis;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyRecommendationAnalytics;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetJobPostingPerformance;

namespace Vettingo.AnalyticsService.API.Controllers
{
    [Route("api/analytics")]
    [ApiController]
    public class AnalyticsController(IMediator mediator) : ControllerBase
    {
        [HttpPost("recommendations")]
        public async Task<IActionResult> RecordCandidateRecommendation([FromBody] RecordCandidateRecommendationCommandRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpGet("companies/{companyId:guid}/recommendations")]
        public async Task<IActionResult> GetCompanyRecommendationAnalytics([FromRoute] GetCompanyRecommendationAnalyticsQueryRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpPost("job-postings/performance")]
        public async Task<IActionResult> RecordJobPostingPerformance([FromBody] RecordJobPostingPerformanceCommandRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpGet("job-postings/{jobPostingId:guid}/performance")]
        public async Task<IActionResult> GetJobPostingPerformance([FromRoute] GetJobPostingPerformanceQueryRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpPost("candidates/cv-analysis")]
        public async Task<IActionResult> RecordCandidateCvAnalysis([FromBody] RecordCandidateCvAnalysisCommandRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpGet("candidates/{candidateId:guid}/cv-analysis")]
        public async Task<IActionResult> GetCandidateCvAnalysis(
            [FromRoute] Guid candidateId,
            [FromQuery] GetCandidateCvAnalysisQueryRequest request)
        {
            return Ok(await mediator.Send(new GetCandidateCvAnalysisQueryRequest(candidateId,latestOnly:request.LatestOnly)));
        }
    }
}
