using FlashMediator;
using Microsoft.AspNetCore.Mvc;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Command.CreateInterviewExam;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Command.DeleteInterviewExam;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Command.UpdateInterviewExam;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Query.GetAll;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Query.GetById;

namespace Vettingo.InterviewService.API.Controllers
{
    [Route("api/interview-exams")]
    [ApiController]
    public class InterviewExamController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllInterviewExams([FromQuery] GetAllInterviewExamsQueryRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpGet("{interviewExamId:guid}")]
        public async Task<IActionResult> GetInterviewExamById([FromRoute] GetInterviewExamByIdQueryRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpPost]
        public async Task<IActionResult> CreateInterviewExam([FromBody] CreateInterviewExamCommandRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }

        [HttpPut("{interviewExamId:guid}")]
        public async Task<IActionResult> UpdateInterviewExam([FromRoute] Guid interviewExamId, [FromBody] UpdateInterviewExamCommandRequest request)
        {
            await mediator.Send(new UpdateInterviewExamCommandRequest
            {
                InterviewExamId = interviewExamId,
                Title = request.Title,
                Description = request.Description,
                QuestionIds = request.QuestionIds
            });

            return Ok();
        }

        [HttpDelete("{interviewExamId:guid}")]
        public async Task<IActionResult> DeleteInterviewExam([FromRoute] Guid interviewExamId)
        {
            await mediator.Send(new DeleteInterviewExamCommandRequest { InterviewExamId = interviewExamId });
            return Ok();
        }
    }
}
