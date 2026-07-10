using FlashMediator;
using Microsoft.AspNetCore.Mvc;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Command.CreateInterviewQuestion;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Command.DeleteInterviewQuestion;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Command.UpdateInterviewQuestion;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Query.GetAll;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Query.GetById;

namespace Vettingo.InterviewService.API.Controllers
{
    [Route("api/interview-questions")]
    [ApiController]
    public class InterviewQuestionController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllInterviewQuestions([FromQuery] GetAllInterviewQuestionsQueryRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpGet("{interviewQuestionId:guid}")]
        public async Task<IActionResult> GetInterviewQuestionById([FromRoute] GetInterviewQuestionByIdQueryRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpPost]
        public async Task<IActionResult> CreateInterviewQuestion([FromBody] CreateInterviewQuestionCommandRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }

        [HttpPut("{interviewQuestionId:guid}")]
        public async Task<IActionResult> UpdateInterviewQuestion([FromRoute] Guid interviewQuestionId, [FromBody] UpdateInterviewQuestionCommandRequest request)
        {
            await mediator.Send(new UpdateInterviewQuestionCommandRequest
            {
                InterviewQuestionId = interviewQuestionId,
                CompanyId = request.CompanyId,
                QuestionText = request.QuestionText
            });

            return Ok();
        }

        [HttpDelete("{interviewQuestionId:guid}")]
        public async Task<IActionResult> DeleteInterviewQuestion([FromRoute] Guid interviewQuestionId)
        {
            await mediator.Send(new DeleteInterviewQuestionCommandRequest { InterviewQuestionId = interviewQuestionId });
            return Ok();
        }
    }
}
