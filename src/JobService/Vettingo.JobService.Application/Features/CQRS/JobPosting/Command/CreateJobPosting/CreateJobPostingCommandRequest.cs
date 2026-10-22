using FlashMediator;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CreateJobPosting
{
    public record CreateJobPostingCommandRequest : IRequest
    {
        public Guid CompanyId { get; init; }
        public string Title { get; init; } = string.Empty;
        public Dictionary<string, int> RequirementSkills { get; init; } = new();
        public int CityId { get; init; }
        public EmploymentType EmploymentType { get; init; }
        public WorkingModel WorkingModel { get; init; }
        public int MinimumExperienceYear { get; init; }
        public int MaximumExperienceYear { get; init; }
        public int MinSalary { get; init; }
        public int MaxSalary { get; init; }
        public DateTime? ApplicationDeadline { get; init; }
        public JobPostingStatus Status { get; init; } = JobPostingStatus.Active;
    }
}
