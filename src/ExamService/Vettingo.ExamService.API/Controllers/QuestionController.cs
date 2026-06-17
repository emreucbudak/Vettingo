using FlashMediator;
using Microsoft.AspNetCore.Mvc;
using Vettingo.ExamService.Application.Features.CQRS.ClassicQuestion.Command.CreateClassicQuestion;
using Vettingo.ExamService.Application.Features.CQRS.ClassicQuestion.Command.DeleteClassicQuestion;
using Vettingo.ExamService.Application.Features.CQRS.ClassicQuestion.Command.UpdateClassicQuestion;
using Vettingo.ExamService.Application.Features.CQRS.ClassicQuestion.Query.GetByExam;
using Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Command.CreateCodeCompletionQuestion;
using Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Command.DeleteCodeCompletionQuestion;
using Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Command.UpdateCodeCompletionQuestion;
using Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Query.GetByExam;
using Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.CreateMultipleChoiceQuestion;
using Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.DeleteMultipleChoiceQuestion;
using Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.UpdateMultipleChoiceQuestion;
using Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Query.GetByExam;
using Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.CreateTrueFalseQuestion;
using Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.DeleteTrueFalseQuestion;
using Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.UpdateTrueFalseQuestion;
using Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Query.GetByExam;

namespace Vettingo.ExamService.API.Controllers
{
    [Route("api/exams/{examId:guid}/questions")]
    [ApiController]
    public class QuestionController(IMediator mediator) : ControllerBase
    {
        [HttpGet("multiple-choice")]
        public async Task<IActionResult> GetMultipleChoiceQuestionsByExamId([FromRoute] Guid examId)
        {
            return Ok(await mediator.Send(new GetMultipleChoiceQuestionsByExamQueryRequest { ExamId = examId }));
        }

        [HttpPost("multiple-choice")]
        public async Task<IActionResult> CreateMultipleChoiceQuestion([FromRoute] Guid examId, [FromBody] CreateMultipleChoiceQuestionCommandRequest request)
        {
            await mediator.Send(new CreateMultipleChoiceQuestionCommandRequest
            {
                ExamId = examId,
                QuestionText = request.QuestionText,
                Topic = request.Topic,
                Point = request.Point,
                Weight = request.Weight,
                DisplayOrder = request.DisplayOrder,
                Explanation = request.Explanation,
                Options = request.Options
            });
            return Ok();
        }

        [HttpPut("multiple-choice/{questionId:guid}")]
        public async Task<IActionResult> UpdateMultipleChoiceQuestion([FromRoute] Guid questionId, [FromBody] UpdateMultipleChoiceQuestionCommandRequest request)
        {
            await mediator.Send(new UpdateMultipleChoiceQuestionCommandRequest
            {
                QuestionId = questionId,
                QuestionText = request.QuestionText,
                Topic = request.Topic,
                Point = request.Point,
                Weight = request.Weight,
                DisplayOrder = request.DisplayOrder,
                Explanation = request.Explanation,
                Options = request.Options
            });

            return Ok();
        }

        [HttpDelete("multiple-choice/{questionId:guid}")]
        public async Task<IActionResult> DeleteMultipleChoiceQuestion([FromRoute] Guid questionId)
        {
            await mediator.Send(new DeleteMultipleChoiceQuestionCommandRequest { QuestionId = questionId });
            return Ok();
        }

        [HttpGet("true-false")]
        public async Task<IActionResult> GetTrueFalseQuestionsByExamId([FromRoute] Guid examId)
        {
            return Ok(await mediator.Send(new GetTrueFalseQuestionsByExamQueryRequest { ExamId = examId }));
        }

        [HttpPost("true-false")]
        public async Task<IActionResult> CreateTrueFalseQuestion([FromRoute] Guid examId, [FromBody] CreateTrueFalseQuestionCommandRequest request)
        {
            await mediator.Send(new CreateTrueFalseQuestionCommandRequest
            {
                ExamId = examId,
                QuestionText = request.QuestionText,
                Topic = request.Topic,
                Point = request.Point,
                Weight = request.Weight,
                DisplayOrder = request.DisplayOrder,
                Explanation = request.Explanation,
                CorrectAnswer = request.CorrectAnswer
            });
            return Ok();
        }

        [HttpPut("true-false/{questionId:guid}")]
        public async Task<IActionResult> UpdateTrueFalseQuestion([FromRoute] Guid questionId, [FromBody] UpdateTrueFalseQuestionCommandRequest request)
        {
            await mediator.Send(new UpdateTrueFalseQuestionCommandRequest
            {
                QuestionId = questionId,
                QuestionText = request.QuestionText,
                Topic = request.Topic,
                Point = request.Point,
                Weight = request.Weight,
                DisplayOrder = request.DisplayOrder,
                Explanation = request.Explanation,
                CorrectAnswer = request.CorrectAnswer
            });
            return Ok();
        }

        [HttpDelete("true-false/{questionId:guid}")]
        public async Task<IActionResult> DeleteTrueFalseQuestion([FromRoute] Guid questionId)
        {
            await mediator.Send(new DeleteTrueFalseQuestionCommandRequest { QuestionId = questionId });
            return Ok();
        }

        [HttpGet("classic")]
        public async Task<IActionResult> GetClassicQuestionsByExamId([FromRoute] Guid examId)
        {
            return Ok(await mediator.Send(new GetClassicQuestionsByExamQueryRequest { ExamId = examId }));
        }

        [HttpPost("classic")]
        public async Task<IActionResult> CreateClassicQuestion([FromRoute] Guid examId, [FromBody] CreateClassicQuestionCommandRequest request)
        {
            await mediator.Send(new CreateClassicQuestionCommandRequest
            {
                ExamId = examId,
                QuestionText = request.QuestionText,
                Topic = request.Topic,
                Point = request.Point,
                Weight = request.Weight,
                DisplayOrder = request.DisplayOrder,
                Explanation = request.Explanation,
                ExpectedAnswer = request.ExpectedAnswer
            });
            return Ok();
        }

        [HttpPut("classic/{questionId:guid}")]
        public async Task<IActionResult> UpdateClassicQuestion([FromRoute] Guid questionId, [FromBody] UpdateClassicQuestionCommandRequest request)
        {
            await mediator.Send(new UpdateClassicQuestionCommandRequest
            {
                QuestionId = questionId,
                QuestionText = request.QuestionText,
                Topic = request.Topic,
                Point = request.Point,
                Weight = request.Weight,
                DisplayOrder = request.DisplayOrder,
                Explanation = request.Explanation,
                ExpectedAnswer = request.ExpectedAnswer
            });
            return Ok();
        }

        [HttpDelete("classic/{questionId:guid}")]
        public async Task<IActionResult> DeleteClassicQuestion([FromRoute] Guid questionId)
        {
            await mediator.Send(new DeleteClassicQuestionCommandRequest { QuestionId = questionId });
            return Ok();
        }

        [HttpGet("code-completion")]
        public async Task<IActionResult> GetCodeCompletionQuestionsByExamId([FromRoute] Guid examId)
        {
            return Ok(await mediator.Send(new GetCodeCompletionQuestionsByExamQueryRequest { ExamId = examId }));
        }

        [HttpPost("code-completion")]
        public async Task<IActionResult> CreateCodeCompletionQuestion([FromRoute] Guid examId, [FromBody] CreateCodeCompletionQuestionCommandRequest request)
        {
            await mediator.Send(new CreateCodeCompletionQuestionCommandRequest
            {
                ExamId = examId,
                QuestionText = request.QuestionText,
                Topic = request.Topic,
                Point = request.Point,
                Weight = request.Weight,
                DisplayOrder = request.DisplayOrder,
                Explanation = request.Explanation,
                CodeSnippet = request.CodeSnippet,
                ExpectedAnswer = request.ExpectedAnswer
            });
            return Ok();
        }

        [HttpPut("code-completion/{questionId:guid}")]
        public async Task<IActionResult> UpdateCodeCompletionQuestion([FromRoute] Guid questionId, [FromBody] UpdateCodeCompletionQuestionCommandRequest request)
        {
            await mediator.Send(new UpdateCodeCompletionQuestionCommandRequest
            {
                QuestionId = questionId,
                QuestionText = request.QuestionText,
                Topic = request.Topic,
                Point = request.Point,
                Weight = request.Weight,
                DisplayOrder = request.DisplayOrder,
                Explanation = request.Explanation,
                CodeSnippet = request.CodeSnippet,
                ExpectedAnswer = request.ExpectedAnswer
            });
            return Ok();
        }

        [HttpDelete("code-completion/{questionId:guid}")]
        public async Task<IActionResult> DeleteCodeCompletionQuestion([FromRoute] Guid questionId)
        {
            await mediator.Send(new DeleteCodeCompletionQuestionCommandRequest { QuestionId = questionId });
            return Ok();
        }
    }
}
