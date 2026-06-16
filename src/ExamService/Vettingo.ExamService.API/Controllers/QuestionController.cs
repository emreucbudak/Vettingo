using FlashMediator;
using Microsoft.AspNetCore.Mvc;
using Vettingo.ExamService.Application.Features.CQRS.Question.Command.CreateQuestion;
using Vettingo.ExamService.Application.Features.CQRS.Question.Command.DeleteQuestion;
using Vettingo.ExamService.Application.Features.CQRS.Question.Command.UpdateQuestion;
using Vettingo.ExamService.Application.Features.CQRS.Question.Query.GetByExam;

namespace Vettingo.ExamService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController(IMediator mediator) : ControllerBase
    {
        [HttpGet("exam/{examId:guid}")]
        public async Task<IActionResult> GetQuestionsByExamId([FromRoute] Guid examId)
        {
            return Ok(await mediator.Send(new GetQuestionsByExamQueryRequest { ExamId = examId }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionCommandRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }

        [HttpPut("{questionId:guid}")]
        public async Task<IActionResult> UpdateQuestion([FromRoute] Guid questionId, [FromBody] UpdateQuestionCommandRequest request)
        {
            await mediator.Send(new UpdateQuestionCommandRequest
            {
                QuestionId = questionId,
                QuestionText = request.QuestionText,
                Topic = request.Topic,
                QuestionType = request.QuestionType,
                Point = request.Point,
                DisplayOrder = request.DisplayOrder,
                Explanation = request.Explanation,
                Options = request.Options
            });

            return Ok();
        }

        [HttpDelete("{questionId:guid}")]
        public async Task<IActionResult> DeleteQuestion([FromRoute] Guid questionId)
        {
            await mediator.Send(new DeleteQuestionCommandRequest { QuestionId = questionId });
            return Ok();
        }
    }
}
