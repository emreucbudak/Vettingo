using Vettingo.ExamService.Domain.Common;

namespace Vettingo.ExamService.Domain.Entities
{
    public class JobExam : BaseEntity
    {
        public Guid JobId { get; set; }
        public Guid ExamId { get; set; }
        public Exam Exam { get; set; } = null!;

        public void CreateJobExam(Guid jobId, Guid examId)
        {
            SetId();
            SetCreatedAt();
            JobId = jobId;
            ExamId = examId;
        }
    }
}
