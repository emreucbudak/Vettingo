using FlashMediator;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Company,Candidate")]
        [HttpGet]
        public async Task<IActionResult> GetAllInterviewExams([FromQuery] GetAllInterviewExamsQueryRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [Authorize(Roles = "Company,Candidate")]
        [HttpGet("{interviewExamId:guid}")]
        public async Task<IActionResult> GetInterviewExamById([FromRoute] GetInterviewExamByIdQueryRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [Authorize(Roles = "Company")]
        [HttpPost]
        public async Task<IActionResult> CreateInterviewExam([FromBody] CreateInterviewExamCommandRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }

        [Authorize(Roles = "Company")]
        [HttpPut("{interviewExamId:guid}")]
        public async Task<IActionResult> UpdateInterviewExam([FromRoute] Guid interviewExamId, [FromBody] UpdateInterviewExamCommandRequest request)
        {
            await mediator.Send(new UpdateInterviewExamCommandRequest
            {
                InterviewExamId = interviewExamId,
                CandidateId = request.CandidateId,
                Title = request.Title,
                Description = request.Description,
                Type = request.Type,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                QuestionIds = request.QuestionIds
            });

            return Ok();
        }

        [Authorize(Roles = "Company")]
        [HttpDelete("{interviewExamId:guid}")]
        public async Task<IActionResult> DeleteInterviewExam([FromRoute] Guid interviewExamId)
        {
            await mediator.Send(new DeleteInterviewExamCommandRequest { InterviewExamId = interviewExamId });
            return Ok();
        }
    }
}
