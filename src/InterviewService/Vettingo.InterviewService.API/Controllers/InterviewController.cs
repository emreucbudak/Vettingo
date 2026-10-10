using System.Security.Claims;
using FlashMediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetByDate;
using Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetCompanyStatistics;
using Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetToday;

namespace Vettingo.InterviewService.API.Controllers
{
    [Route("api/interviews")]
    [ApiController]
    public class InterviewController(IMediator mediator) : ControllerBase
    {
        [Authorize(Roles = "Company,Human Resources")]
        [HttpGet("company/by-date")]
        public async Task<IActionResult> GetCompanyInterviewsByDate([FromQuery] DateOnly date, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirstValue("companyId"), out var companyId) || companyId == Guid.Empty)
                return Unauthorized();

            if (date == default)
                return BadRequest("Mülakat tarihi gereklidir.");

            return Ok(await mediator.Send(new GetInterviewsByDateQueryRequest(companyId, date), cancellationToken));
        }

        [Authorize(Roles = "Company,Human Resources")]
        [HttpGet("company/statistics")]
        public async Task<IActionResult> GetCompanyStatistics(CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirstValue("companyId"), out var companyId) || companyId == Guid.Empty)
                return Unauthorized();

            return Ok(await mediator.Send(new GetCompanyInterviewStatisticsQueryRequest(companyId), cancellationToken));
        }

        [Authorize(Roles = "Company,Human Resources")]
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayInterviews(CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirstValue("companyId"), out var companyId) || companyId == Guid.Empty)
                return Unauthorized();

            return Ok(await mediator.Send(new GetTodayInterviewsQueryRequest(companyId), cancellationToken));
        }
    }
}
