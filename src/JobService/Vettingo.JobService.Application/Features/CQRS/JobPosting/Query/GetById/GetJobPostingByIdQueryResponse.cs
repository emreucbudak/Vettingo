using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetById
{
    public class GetJobPostingByIdQueryResponse
    {
        public Guid Id { get; init; }
        public Guid CompanyId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string Requirements { get; init; } = string.Empty;
        public string Responsibilities { get; init; } = string.Empty;
        public string Location { get; init; } = string.Empty;
        public EmploymentType EmploymentType { get; init; }
        public WorkingModel WorkingModel { get; init; }
        public ExperienceLevel ExperienceLevel { get; init; }
        public decimal? MinSalary { get; init; }
        public decimal? MaxSalary { get; init; }
        public DateTime? ApplicationDeadline { get; init; }
        public JobPostingStatus Status { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
