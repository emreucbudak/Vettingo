using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Domain.Common;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Domain.Entities
{
    [Index(nameof(Title), IsUnique = true)]
    public class JobPosting : BaseEntity
    {
        public JobPosting()
        {
        }

        public Guid CompanyId { get; private set; }
        public string CompanyName { get; private set; } = string.Empty;
        public string Title { get; private set; } = string.Empty;
        public Dictionary<string, int> RequirementSkills { get; private set; } = new();
        public int CityId { get; private set; }
        public City City { get; private set; } = null!;
        public EmploymentType EmploymentType { get; private set; }
        public WorkingModel WorkingModel { get; private set; }
        public int MinimumExperienceYear { get; private set; }
        public int MaximumExperienceYear { get; private set; }
        public int MinSalary { get; private set; }
        public int MaxSalary { get; private set; }
        public DateTime? ApplicationDeadline { get; private set; }
        public JobPostingStatus Status { get; private set; }

        public void CreateJobPosting(
            Guid companyId,
            string companyName,
            string title,
            Dictionary<string, int> requirementSkills,
            int cityId,
            EmploymentType employmentType,
            WorkingModel workingModel,
            int minimumExperienceYear,
            int maximumExperienceYear,
            int minSalary,
            int maxSalary,
            DateTime? applicationDeadline,
            JobPostingStatus status)
        {
            CheckJobPostingContent(companyId, title, cityId, employmentType, workingModel, applicationDeadline, status);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companyName, nameof(companyName));
            SetId();
            CompanyId = companyId;
            CompanyName = companyName;
            UpdateJobPosting(title, requirementSkills, cityId, employmentType, workingModel, minimumExperienceYear, maximumExperienceYear, minSalary, maxSalary, applicationDeadline);
            Status = status;
            SetCreatedAt();
            UpdatedAt = null;
        }

        public void UpdateJobPosting(
            string title,
            Dictionary<string, int> requirementSkills,
            int cityId,
            EmploymentType employmentType,
            WorkingModel workingModel,
            int minimumExperienceYear,
            int maximumExperienceYear,
            int minSalary,
            int maxSalary,
            DateTime? applicationDeadline)
        {
            CheckJobPostingContent(CompanyId, title, cityId, employmentType, workingModel, applicationDeadline);
            Title = title;
            RequirementSkills = new Dictionary<string, int>(requirementSkills);
            CityId = cityId;
            EmploymentType = employmentType;
            WorkingModel = workingModel;
            MinimumExperienceYear = minimumExperienceYear;
            MaximumExperienceYear = maximumExperienceYear;
            MinSalary = minSalary;
            MaxSalary = maxSalary;
            ApplicationDeadline = applicationDeadline;
            SetUpdatedAt();
        }

        public void SetStatus(JobPostingStatus status)
        {
            if (!Enum.IsDefined(typeof(JobPostingStatus), status))
            {
                throw new ArgumentOutOfRangeException(nameof(status), status, "İş ilanı durumu geçersiz.");
            }

            Status = status;
            SetUpdatedAt();
        }

        public void CheckJobPosting(Guid companyId, string title)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentException("CompanyId boş olamaz.", nameof(companyId));
            }

            ArgumentNullException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        }

        public void CheckJobPostingContent(
            Guid companyId,
            string title,
            int cityId,
            EmploymentType employmentType,
            WorkingModel workingModel,
            DateTime? applicationDeadline,
            JobPostingStatus? status = null)
        {
            CheckJobPosting(companyId, title);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(cityId, nameof(cityId));

            if (!Enum.IsDefined(typeof(EmploymentType), employmentType))
            {
                throw new ArgumentOutOfRangeException(nameof(employmentType), employmentType, "Çalışma tipi geçersiz.");
            }

            if (!Enum.IsDefined(typeof(WorkingModel), workingModel))
            {
                throw new ArgumentOutOfRangeException(nameof(workingModel), workingModel, "Çalışma modeli geçersiz.");
            }

            if (status.HasValue && !Enum.IsDefined(typeof(JobPostingStatus), status.Value))
            {
                throw new ArgumentOutOfRangeException(nameof(status), status.Value, "İş ilanı durumu geçersiz.");
            }

            if (applicationDeadline.HasValue && applicationDeadline.Value == default)
            {
                throw new ArgumentException("Son başvuru tarihi geçersiz.", nameof(applicationDeadline));
            }
        }
    }
}
