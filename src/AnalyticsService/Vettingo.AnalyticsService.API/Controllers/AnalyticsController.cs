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
        public async Task<IActionResult> GetCompanyRecommendationAnalytics([FromRoute] Guid companyId)
        {
            return Ok(await mediator.Send(new GetCompanyRecommendationAnalyticsQueryRequest { CompanyId = companyId }));
        }

        [HttpPost("job-postings/performance")]
        public async Task<IActionResult> RecordJobPostingPerformance([FromBody] RecordJobPostingPerformanceCommandRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpGet("job-postings/{jobPostingId:guid}/performance")]
        public async Task<IActionResult> GetJobPostingPerformance([FromRoute] Guid jobPostingId)
        {
            return Ok(await mediator.Send(new GetJobPostingPerformanceQueryRequest { JobPostingId = jobPostingId }));
        }

        [HttpPost("candidates/cv-analysis")]
        public async Task<IActionResult> RecordCandidateCvAnalysis([FromBody] RecordCandidateCvAnalysisCommandRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpGet("candidates/{candidateId:guid}/cv-analysis")]
        public async Task<IActionResult> GetCandidateCvAnalysis([FromRoute] Guid candidateId, [FromQuery] bool latestOnly = false)
        {
            return Ok(await mediator.Send(new GetCandidateCvAnalysisQueryRequest { CandidateId = candidateId, LatestOnly = latestOnly }));
        }
    }
}
