using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetAll
{
    public sealed record GetAllJobPostingsQueryResponse
    {
        public Guid Id { get; init; }
        public Guid CompanyId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public int CityId { get; init; }
        public EmploymentType EmploymentType { get; init; }
        public WorkingModel WorkingModel { get; init; }
        public int MinimumExperienceYear { get; init; }
        public int MaximumExperienceYear { get; init; }
        public int Salary { get; init; }
        public DateTime? ApplicationDeadline { get; init; }
        public JobPostingStatus Status { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
