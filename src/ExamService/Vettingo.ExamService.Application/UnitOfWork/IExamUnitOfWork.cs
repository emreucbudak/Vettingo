using Vettingo.ExamService.Application.Repository;
using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.Application.UnitOfWork;

public interface IExamUnitOfWork : IAsyncDisposable
{
    IExamRepository<JobStageExam> JobStageExamRepository { get; }
    IExamRepository<ExamAttempt> ExamAttemptRepository { get; }

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task CommitTransactionAsync(CancellationToken cancellationToken);
    Task RollbackTransactionAsync();
}
