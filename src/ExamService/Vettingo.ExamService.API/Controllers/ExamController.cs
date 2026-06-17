using FlashMediator;
using Microsoft.AspNetCore.Mvc;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Command.CreateExam;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Command.DeleteExam;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Command.UpdateExam;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetAll;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetById;

namespace Vettingo.ExamService.API.Controllers
{
    [Route("api/exams")]
    [ApiController]
    public class ExamController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllExams()
        {
            return Ok(await mediator.Send(new GetAllExamsQueryRequest()));
        }

        [HttpGet("{examId:guid}")]
        public async Task<IActionResult> GetExamById([FromRoute] Guid examId)
        {
            return Ok(await mediator.Send(new GetExamByIdQueryRequest { ExamId = examId }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateExam([FromBody] CreateExamCommandRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }

        [HttpPut("{examId:guid}")]
        public async Task<IActionResult> UpdateExam([FromRoute] Guid examId, [FromBody] UpdateExamCommandRequest request)
        {
            await mediator.Send(new UpdateExamCommandRequest
            {
                ExamId = examId,
                CompanyId = request.CompanyId,
                JobId = request.JobId,
                OwnerType = request.OwnerType,
                Title = request.Title,
                Subject = request.Subject,
                Description = request.Description,
                DurationMinutes = request.DurationMinutes,
                PassingScore = request.PassingScore,
                IsActive = request.IsActive
            });

            return Ok();
        }

        [HttpDelete("{examId:guid}")]
        public async Task<IActionResult> DeleteExam([FromRoute] Guid examId)
        {
            await mediator.Send(new DeleteExamCommandRequest { ExamId = examId });
            return Ok();
        }
    }
}
