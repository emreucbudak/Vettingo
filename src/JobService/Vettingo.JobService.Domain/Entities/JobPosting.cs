using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Domain.Common;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Domain.Entities
{
    [Index(nameof(Title))]
    public class JobPosting : BaseEntity
    {
        private List<JobSteps> _jobSteps = new();
        private readonly List<CandidateSkill> _candidateSkills = new();

        public JobPosting()
        {
        }

        public Guid CompanyId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public int CityId { get; private set; }
        public City City { get; private set; } = null!;
        public EmploymentType EmploymentType { get; private set; }
        public WorkingModel WorkingModel { get; private set; }
        public int MinimumExperienceYear { get; private set; }
        public int MaximumExperienceYear { get; private set; }
        public int MinSalary { get; private set; }
        public int MaxSalary { get; private set; }
        public int CurrentStep { get; private set; }
        public bool IsApplicationClosed { get; private set; }
        public bool IsPostingClosed { get; private set; }
        public IReadOnlyCollection<JobSteps> JobSteps => _jobSteps.AsReadOnly();
        public IReadOnlyCollection<CandidateSkill> CandidateSkills => _candidateSkills.AsReadOnly();

        public StepType? GetCurrentStepType() => JobSteps.FirstOrDefault(step => step.Order == CurrentStep)?.StepType;

        public void CreateJobPosting(
            Guid companyId,
            string title,
            int cityId,
            EmploymentType employmentType,
            WorkingModel workingModel,
            int minimumExperienceYear,
            int maximumExperienceYear,
            int minSalary,
            int maxSalary,
            List<JobSteps> jobSteps)
        {
            CheckJobPostingContent(companyId, title, cityId, employmentType, workingModel);
            ArgumentNullException.ThrowIfNull(jobSteps, nameof(jobSteps));
            if (jobSteps.Count == 0 || jobSteps.Any(step => step is null))
            {
                throw new ArgumentException("İş ilanı için en az bir adım gereklidir.", nameof(jobSteps));
            }

            SetId();
            CompanyId = companyId;
            Title = title;
            CityId = cityId;
            EmploymentType = employmentType;
            WorkingModel = workingModel;
            MinimumExperienceYear = minimumExperienceYear;
            MaximumExperienceYear = maximumExperienceYear;
            MinSalary = minSalary;
            MaxSalary = maxSalary;
            CurrentStep = 1;
            IsApplicationClosed = false;
            IsPostingClosed = false;
            _jobSteps = jobSteps;
            SetCreatedAt();
            UpdatedAt = null;
        }

        public void AddCandidateSkill(Skill skill, int experienceYear)
        {
            var candidateSkill = new CandidateSkill();
            candidateSkill.CreateCandidateSkill(Id, skill, experienceYear);
            _candidateSkills.Add(candidateSkill);
        }

        public void AdvanceCurrentStep()
        {
            CurrentStep++;
            SetUpdatedAt();
        }

        public void CloseJobPosting()
        {
            IsPostingClosed = true;
            SetUpdatedAt();
        }

        public void CloseJobPostingApplications()
        {
            IsApplicationClosed = true;
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
            WorkingModel workingModel)
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
        }
    }
}
