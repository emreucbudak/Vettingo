using System.Security.Claims;
using FlashMediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Command.CreateExam;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Command.DeleteExam;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Command.UpdateExam;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetByCompanyId;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetById;

namespace Vettingo.ExamService.API.Controllers
{
    [Route("api/exams")]
    [ApiController]
    public class ExamController(IMediator mediator) : ControllerBase
    {
        [Authorize(Roles = "Company,Human Resources")]
        [HttpGet("company")]
        public async Task<IActionResult> GetExamsByCompanyId(CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirstValue("companyId"), out var companyId) || companyId == Guid.Empty)
            {
                return Unauthorized();
            }

            return Ok(await mediator.Send(new GetExamsByCompanyIdQueryRequest(companyId), cancellationToken));
        }

        [Authorize(Roles = "Company")]
        [HttpGet("{examId:guid}")]
        public async Task<IActionResult> GetExamById([FromRoute] Guid examId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirstValue("companyId"), out var companyId) || companyId == Guid.Empty)
            {
                return Unauthorized();
            }

            var request = new GetExamByIdQueryRequest(examId, companyId);
            return Ok(await mediator.Send(request, cancellationToken));
        }

        [Authorize(Roles = "Company")]
        [HttpPost]
        public async Task<IActionResult> CreateExam([FromBody] CreateExamCommandRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }

        [Authorize(Roles = "Company")]
        [HttpPut("{examId:guid}")]
        public async Task<IActionResult> UpdateExam(
            [FromRoute] Guid examId, [FromBody] UpdateExamCommandRequest request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirstValue("companyId"), out var companyId) || companyId == Guid.Empty)
            {
                return Unauthorized();
            }

            await mediator.Send(request with { ExamId = examId, CompanyId = companyId }, cancellationToken);
            return Ok();
        }

        [Authorize(Roles = "Company")]
        [HttpDelete("{examId:guid}")]
        public async Task<IActionResult> DeleteExam([FromRoute] Guid examId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirstValue("companyId"), out var companyId) || companyId == Guid.Empty)
            {
                return Unauthorized();
            }

            await mediator.Send(new DeleteExamCommandRequest { ExamId = examId, CompanyId = companyId }, cancellationToken);
            return Ok();
        }
    }
}
