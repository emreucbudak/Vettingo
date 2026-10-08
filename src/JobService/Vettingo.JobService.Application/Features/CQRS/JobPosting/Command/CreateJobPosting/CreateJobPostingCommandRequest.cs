using FlashMediator;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CreateJobPosting
{
    public record CreateJobPostingCommandRequest : IRequest
    {
        public Guid CompanyId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string Requirements { get; init; } = string.Empty;
        public string Responsibilities { get; init; } = string.Empty;
        public int CityId { get; init; }
        public EmploymentType EmploymentType { get; init; }
        public WorkingModel WorkingModel { get; init; }
        public ExperienceLevel ExperienceLevel { get; init; }
        public int Salary { get; init; }
        public DateTime? ApplicationDeadline { get; init; }
        public JobPostingStatus Status { get; init; } = JobPostingStatus.Active;
    }
}
