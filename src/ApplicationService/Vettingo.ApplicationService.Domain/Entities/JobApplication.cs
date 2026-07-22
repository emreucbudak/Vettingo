using Vettingo.ApplicationService.Domain.Common;
using Vettingo.ApplicationService.Domain.Enums;

namespace Vettingo.ApplicationService.Domain.Entities
{
    public class JobApplication : BaseEntity
    {
        public Guid CandidateId { get; private set; }
        public Guid JobPostingId { get; private set; }
        public DateTime AppliedAt { get; private set; }
        public ApplicationStatus Status { get; private set; }

        public void CreateApplication(
            Guid candidateId,
            Guid jobPostingId,
            DateTime appliedAt,
            ApplicationStatus status)
        {
            ValidateGuid(candidateId, nameof(candidateId));
            ValidateGuid(jobPostingId, nameof(jobPostingId));
            ValidateAppliedAt(appliedAt);
            ValidateStatus(status);

            SetId();
            CandidateId = candidateId;
            JobPostingId = jobPostingId;
            AppliedAt = appliedAt.ToUniversalTime();
            Status = status;
            SetCreatedAt();
            UpdatedAt = null;
        }

        public void UpdateStatus(ApplicationStatus status)
        {
            ValidateStatus(status);
            Status = status;
            SetUpdatedAt();
        }

        private static void ValidateGuid(Guid value, string parameterName)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{parameterName} boş olamaz.", parameterName);
            }
        }

        private static void ValidateAppliedAt(DateTime appliedAt)
        {
            if (appliedAt == default)
            {
                throw new ArgumentException("Başvuru tarihi geçersiz.", nameof(appliedAt));
            }
        }

        private static void ValidateStatus(ApplicationStatus status)
        {
            if (!Enum.IsDefined(status))
            {
                throw new ArgumentOutOfRangeException(nameof(status), status, "Başvuru durumu geçersiz.");
            }
        }
    }
}
