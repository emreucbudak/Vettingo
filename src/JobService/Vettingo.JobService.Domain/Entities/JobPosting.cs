using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Domain.Entities
{
    [Index(nameof(Title), IsUnique = true)]
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
        public int CityId { get; private set; }
        public City City { get; private set; } = null!;
        public EmploymentType EmploymentType { get; private set; }
        public WorkingModel WorkingModel { get; private set; }
        public ExperienceLevel ExperienceLevel { get; private set; }
        public int Salary { get; private set; }
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
            int cityId,
            EmploymentType employmentType,
            WorkingModel workingModel,
            ExperienceLevel experienceLevel,
            int salary,
            DateTime? applicationDeadline,
            JobPostingStatus status)
        {
            CheckJobPostingContent(companyId, title, description, requirements, responsibilities, cityId, employmentType, workingModel, experienceLevel, salary, applicationDeadline, status);
            SetId();
            CompanyId = companyId;
            UpdateJobPosting(title, description, requirements, responsibilities, cityId, employmentType, workingModel, experienceLevel, salary, applicationDeadline);
            Status = status;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
        }

        public void UpdateJobPosting(
            string title,
            string description,
            string requirements,
            string responsibilities,
            int cityId,
            EmploymentType employmentType,
            WorkingModel workingModel,
            ExperienceLevel experienceLevel,
            int salary,
            DateTime? applicationDeadline)
        {
            CheckJobPostingContent(CompanyId, title, description, requirements, responsibilities, cityId, employmentType, workingModel, experienceLevel, salary, applicationDeadline);
            Title = title;
            Description = description;
            Requirements = requirements;
            Responsibilities = responsibilities;
            CityId = cityId;
            EmploymentType = employmentType;
            WorkingModel = workingModel;
            ExperienceLevel = experienceLevel;
            Salary = salary;
            ApplicationDeadline = applicationDeadline;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetStatus(JobPostingStatus status)
        {
            if (!Enum.IsDefined(typeof(JobPostingStatus), status))
            {
                throw new ArgumentOutOfRangeException(nameof(status), status, "İş ilanı durumu geçersiz.");
            }

            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public void CheckJobPosting(Guid companyId, string title, string description, string requirements)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentException("CompanyId boş olamaz.", nameof(companyId));
            }

            ArgumentNullException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(description, nameof(description));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(requirements, nameof(requirements));
        }

        public void CheckJobPostingContent(
            Guid companyId,
            string title,
            string description,
            string requirements,
            string responsibilities,
            int cityId,
            EmploymentType employmentType,
            WorkingModel workingModel,
            ExperienceLevel experienceLevel,
            int salary,
            DateTime? applicationDeadline,
            JobPostingStatus? status = null)
        {
            CheckJobPosting(companyId, title, description, requirements);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(responsibilities, nameof(responsibilities));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(cityId, nameof(cityId));

            if (!Enum.IsDefined(typeof(EmploymentType), employmentType))
            {
                throw new ArgumentOutOfRangeException(nameof(employmentType), employmentType, "Çalışma tipi geçersiz.");
            }

            if (!Enum.IsDefined(typeof(WorkingModel), workingModel))
            {
                throw new ArgumentOutOfRangeException(nameof(workingModel), workingModel, "Çalışma modeli geçersiz.");
            }

            if (!Enum.IsDefined(typeof(ExperienceLevel), experienceLevel))
            {
                throw new ArgumentOutOfRangeException(nameof(experienceLevel), experienceLevel, "Deneyim seviyesi geçersiz.");
            }

            if (status.HasValue && !Enum.IsDefined(typeof(JobPostingStatus), status.Value))
            {
                throw new ArgumentOutOfRangeException(nameof(status), status.Value, "İş ilanı durumu geçersiz.");
            }

            ArgumentOutOfRangeException.ThrowIfNegative(salary, nameof(salary));

            if (applicationDeadline.HasValue && applicationDeadline.Value == default)
            {
                throw new ArgumentException("Son başvuru tarihi geçersiz.", nameof(applicationDeadline));
            }
        }
    }
}
