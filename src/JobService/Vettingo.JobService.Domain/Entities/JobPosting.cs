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
            CheckJobPostingContent(companyId, title, description, requirements, responsibilities, location, employmentType, workingModel, experienceLevel, minSalary, maxSalary, applicationDeadline, status);
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
            CheckJobPostingContent(CompanyId, title, description, requirements, responsibilities, location, employmentType, workingModel, experienceLevel, minSalary, maxSalary, applicationDeadline);
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
            string location,
            EmploymentType employmentType,
            WorkingModel workingModel,
            ExperienceLevel experienceLevel,
            decimal? minSalary,
            decimal? maxSalary,
            DateTime? applicationDeadline,
            JobPostingStatus? status = null)
        {
            CheckJobPosting(companyId, title, description, requirements);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(responsibilities, nameof(responsibilities));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(location, nameof(location));

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

            if (minSalary.HasValue && minSalary.Value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minSalary), minSalary, "Minimum maaş negatif olamaz.");
            }

            if (maxSalary.HasValue && maxSalary.Value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSalary), maxSalary, "Maksimum maaş negatif olamaz.");
            }

            if (minSalary.HasValue && maxSalary.HasValue && minSalary.Value > maxSalary.Value)
            {
                throw new ArgumentException("Minimum maaş maksimum maaştan büyük olamaz.", nameof(minSalary));
            }

            if (applicationDeadline.HasValue && applicationDeadline.Value == default)
            {
                throw new ArgumentException("Son başvuru tarihi geçersiz.", nameof(applicationDeadline));
            }
        }
    }
}
