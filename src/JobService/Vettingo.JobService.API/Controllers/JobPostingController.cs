using FlashMediator;
using Microsoft.AspNetCore.Mvc;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CreateJobPosting;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.DeleteJobPosting;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.UpdateJobPosting;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetAll;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetById;

namespace Vettingo.JobService.API.Controllers
{
    [Route("api/job-postings")]
    [ApiController]
    public class JobPostingController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllJobPostings([FromQuery] Guid? companyId)
        {
            return Ok(await mediator.Send(new GetAllJobPostingsQueryRequest { CompanyId = companyId }));
        }

        [HttpGet("{jobPostingId:guid}")]
        public async Task<IActionResult> GetJobPostingById([FromRoute] Guid jobPostingId)
        {
            return Ok(await mediator.Send(new GetJobPostingByIdQueryRequest { JobPostingId = jobPostingId }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateJobPosting([FromBody] CreateJobPostingCommandRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }

        [HttpPut("{jobPostingId:guid}")]
        public async Task<IActionResult> UpdateJobPosting([FromRoute] Guid jobPostingId, [FromBody] UpdateJobPostingCommandRequest request)
        {
            await mediator.Send(new UpdateJobPostingCommandRequest
            {
                JobPostingId = jobPostingId,
                Title = request.Title,
                Description = request.Description,
                Requirements = request.Requirements,
                Responsibilities = request.Responsibilities,
                Location = request.Location,
                EmploymentType = request.EmploymentType,
                WorkingModel = request.WorkingModel,
                ExperienceLevel = request.ExperienceLevel,
                MinSalary = request.MinSalary,
                MaxSalary = request.MaxSalary,
                ApplicationDeadline = request.ApplicationDeadline,
                Status = request.Status
            });

            return Ok();
        }

        [HttpDelete("{jobPostingId:guid}")]
        public async Task<IActionResult> DeleteJobPosting([FromRoute] Guid jobPostingId)
        {
            await mediator.Send(new DeleteJobPostingCommandRequest { JobPostingId = jobPostingId });
            return Ok();
        }
    }
}
