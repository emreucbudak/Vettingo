using FlashMediator;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Company,Candidate")]
        [HttpGet("multiple-choice")]
        public async Task<IActionResult> GetMultipleChoiceQuestionsByExamId([FromRoute] GetMultipleChoiceQuestionsByExamQueryRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [Authorize(Roles = "Company")]
        [HttpPost("multiple-choice")]
        public async Task<IActionResult> CreateMultipleChoiceQuestion([FromRoute] Guid examId, [FromBody] CreateMultipleChoiceQuestionCommandRequest request)
        {
            await mediator.Send(new CreateMultipleChoiceQuestionCommandRequest
            {
                ExamId = examId,
                QuestionText = request.QuestionText,
                Weight = request.Weight,
                DisplayOrder = request.DisplayOrder,
                Explanation = request.Explanation,
                Options = request.Options
            });
            return Ok();
        }

        [Authorize(Roles = "Company")]
        [HttpPut("multiple-choice/{questionId:guid}")]
        public async Task<IActionResult> UpdateMultipleChoiceQuestion([FromRoute] Guid questionId, [FromBody] UpdateMultipleChoiceQuestionCommandRequest request)
        {
            await mediator.Send(new UpdateMultipleChoiceQuestionCommandRequest
            {
                QuestionId = questionId,
                QuestionText = request.QuestionText,
                Weight = request.Weight,
                DisplayOrder = request.DisplayOrder,
                Explanation = request.Explanation,
                Options = request.Options
            });

            return Ok();
        }

        [Authorize(Roles = "Company")]
        [HttpDelete("multiple-choice/{questionId:guid}")]
        public async Task<IActionResult> DeleteMultipleChoiceQuestion([FromRoute] Guid questionId)
        {
            await mediator.Send(new DeleteMultipleChoiceQuestionCommandRequest { QuestionId = questionId });
            return Ok();
        }

        [Authorize(Roles = "Company,Candidate")]
        [HttpGet("true-false")]
        public async Task<IActionResult> GetTrueFalseQuestionsByExamId([FromRoute] GetTrueFalseQuestionsByExamQueryRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [Authorize(Roles = "Company")]
        [HttpPost("true-false")]
        public async Task<IActionResult> CreateTrueFalseQuestion([FromRoute] Guid examId, [FromBody] CreateTrueFalseQuestionCommandRequest request)
        {
            await mediator.Send(new CreateTrueFalseQuestionCommandRequest
            {
                ExamId = examId,
                QuestionText = request.QuestionText,
                Weight = request.Weight,
                DisplayOrder = request.DisplayOrder,
                Explanation = request.Explanation,
                CorrectAnswer = request.CorrectAnswer
            });
            return Ok();
        }

        [Authorize(Roles = "Company")]
        [HttpPut("true-false/{questionId:guid}")]
        public async Task<IActionResult> UpdateTrueFalseQuestion([FromRoute] Guid questionId, [FromBody] UpdateTrueFalseQuestionCommandRequest request)
        {
            await mediator.Send(new UpdateTrueFalseQuestionCommandRequest
            {
                QuestionId = questionId,
                QuestionText = request.QuestionText,
                Weight = request.Weight,
                DisplayOrder = request.DisplayOrder,
                Explanation = request.Explanation,
                CorrectAnswer = request.CorrectAnswer
            });
            return Ok();
        }

        [Authorize(Roles = "Company")]
        [HttpDelete("true-false/{questionId:guid}")]
        public async Task<IActionResult> DeleteTrueFalseQuestion([FromRoute] Guid questionId)
        {
            await mediator.Send(new DeleteTrueFalseQuestionCommandRequest { QuestionId = questionId });
            return Ok();
        }

        [Authorize(Roles = "Company,Candidate")]
        [HttpGet("classic")]
        public async Task<IActionResult> GetClassicQuestionsByExamId([FromRoute] GetClassicQuestionsByExamQueryRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [Authorize(Roles = "Company")]
        [HttpPost("classic")]
        public async Task<IActionResult> CreateClassicQuestion([FromRoute] Guid examId, [FromBody] CreateClassicQuestionCommandRequest request)
        {
            await mediator.Send(new CreateClassicQuestionCommandRequest
            {
                ExamId = examId,
                QuestionText = request.QuestionText,
                Weight = request.Weight,
                DisplayOrder = request.DisplayOrder,
                Explanation = request.Explanation,
                ExpectedAnswer = request.ExpectedAnswer
            });
            return Ok();
        }

        [Authorize(Roles = "Company")]
        [HttpPut("classic/{questionId:guid}")]
        public async Task<IActionResult> UpdateClassicQuestion([FromRoute] Guid questionId, [FromBody] UpdateClassicQuestionCommandRequest request)
        {
            await mediator.Send(new UpdateClassicQuestionCommandRequest
            {
                QuestionId = questionId,
                QuestionText = request.QuestionText,
                Weight = request.Weight,
                DisplayOrder = request.DisplayOrder,
                Explanation = request.Explanation,
                ExpectedAnswer = request.ExpectedAnswer
            });
            return Ok();
        }

        [Authorize(Roles = "Company")]
        [HttpDelete("classic/{questionId:guid}")]
        public async Task<IActionResult> DeleteClassicQuestion([FromRoute] Guid questionId)
        {
            await mediator.Send(new DeleteClassicQuestionCommandRequest { QuestionId = questionId });
            return Ok();
        }

        [Authorize(Roles = "Company,Candidate")]
        [HttpGet("code-completion")]
        public async Task<IActionResult> GetCodeCompletionQuestionsByExamId([FromRoute] GetCodeCompletionQuestionsByExamQueryRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [Authorize(Roles = "Company")]
        [HttpPost("code-completion")]
        public async Task<IActionResult> CreateCodeCompletionQuestion([FromRoute] Guid examId, [FromBody] CreateCodeCompletionQuestionCommandRequest request)
        {
            await mediator.Send(new CreateCodeCompletionQuestionCommandRequest
            {
                ExamId = examId,
                QuestionText = request.QuestionText,
                Weight = request.Weight,
                DisplayOrder = request.DisplayOrder,
                Explanation = request.Explanation,
                CodeSnippet = request.CodeSnippet,
                ExpectedAnswer = request.ExpectedAnswer
            });
            return Ok();
        }

        [Authorize(Roles = "Company")]
        [HttpPut("code-completion/{questionId:guid}")]
        public async Task<IActionResult> UpdateCodeCompletionQuestion([FromRoute] Guid questionId, [FromBody] UpdateCodeCompletionQuestionCommandRequest request)
        {
            await mediator.Send(new UpdateCodeCompletionQuestionCommandRequest
            {
                QuestionId = questionId,
                QuestionText = request.QuestionText,
                Weight = request.Weight,
                DisplayOrder = request.DisplayOrder,
                Explanation = request.Explanation,
                CodeSnippet = request.CodeSnippet,
                ExpectedAnswer = request.ExpectedAnswer
            });
            return Ok();
        }

        [Authorize(Roles = "Company")]
        [HttpDelete("code-completion/{questionId:guid}")]
        public async Task<IActionResult> DeleteCodeCompletionQuestion([FromRoute] Guid questionId)
        {
            await mediator.Send(new DeleteCodeCompletionQuestionCommandRequest { QuestionId = questionId });
            return Ok();
        }
    }
}
