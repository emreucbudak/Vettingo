using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Domain.Entities
{
    public class JobPosting
    {
        public JobPosting()
        {
        }

        public Guid Id { get; private set; }
        public Guid CompanyId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string Requirements { get; private set; } = string.Empty;
        public string Responsibilities { get; private set; } = string.Empty;
        public string Location { get; private set; } = string.Empty;
        public EmploymentType EmploymentType { get; private set; }
        public WorkingModel WorkingModel { get; private set; }
        public ExperienceLevel ExperienceLevel { get; private set; }
        public decimal? MinSalary { get; private set; }
        public decimal? MaxSalary { get; private set; }
        public DateTime? ApplicationDeadline { get; private set; }
        public JobPostingStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        public void CreateJobPosting(
            Guid companyId,
            string title,
            string description,
            string requirements,
            string responsibilities,
            string location,
            EmploymentType employmentType,
            WorkingModel workingModel,
            ExperienceLevel experienceLevel,
            decimal? minSalary,
            decimal? maxSalary,
            DateTime? applicationDeadline,
            JobPostingStatus status)
        {
            SetId();
            CompanyId = companyId;
            UpdateJobPosting(title, description, requirements, responsibilities, location, employmentType, workingModel, experienceLevel, minSalary, maxSalary, applicationDeadline);
            Status = status;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
        }

        public void UpdateJobPosting(
            string title,
            string description,
            string requirements,
            string responsibilities,
            string location,
            EmploymentType employmentType,
            WorkingModel workingModel,
            ExperienceLevel experienceLevel,
            decimal? minSalary,
            decimal? maxSalary,
            DateTime? applicationDeadline)
        {
            Title = title;
            Description = description;
            Requirements = requirements;
            Responsibilities = responsibilities;
            Location = location;
            EmploymentType = employmentType;
            WorkingModel = workingModel;
            ExperienceLevel = experienceLevel;
            MinSalary = minSalary;
            MaxSalary = maxSalary;
            ApplicationDeadline = applicationDeadline;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetStatus(JobPostingStatus status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
