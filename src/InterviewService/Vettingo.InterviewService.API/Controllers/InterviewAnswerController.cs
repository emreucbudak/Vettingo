using FlashMediator;
using Microsoft.AspNetCore.Mvc;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewAnswer.Command.CreateInterviewAnswer;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewAnswer.Query.GetAll;

namespace Vettingo.InterviewService.API.Controllers
{
    [Route("api/interview-answers")]
    [ApiController]
    public class InterviewAnswerController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetInterviewAnswers([FromQuery] Guid? userId, [FromQuery] Guid? interviewExamId)
        {
            return Ok(await mediator.Send(new GetInterviewAnswersQueryRequest { UserId = userId, InterviewExamId = interviewExamId }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateInterviewAnswer([FromBody] CreateInterviewAnswerCommandRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }
    }
}
