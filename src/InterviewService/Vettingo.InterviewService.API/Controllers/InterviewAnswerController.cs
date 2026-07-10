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
        public async Task<IActionResult> GetInterviewAnswers([FromQuery] GetInterviewAnswersQueryRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpPost]
        public async Task<IActionResult> CreateInterviewAnswer([FromBody] CreateInterviewAnswerCommandRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }
    }
}
