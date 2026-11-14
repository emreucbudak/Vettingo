using FlashMediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Query.GetByExam;
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

        [Authorize(Roles = "Company,Candidate")]
        [HttpGet("true-false")]
        public async Task<IActionResult> GetTrueFalseQuestionsByExamId([FromRoute] GetTrueFalseQuestionsByExamQueryRequest request)
        {
            return Ok(await mediator.Send(request));
        }
    }
}
