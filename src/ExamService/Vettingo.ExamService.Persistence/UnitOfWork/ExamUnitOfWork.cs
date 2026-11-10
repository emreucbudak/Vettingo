using DotNetCore.CAP;
using DotNetCore.CAP.Transport;
using Microsoft.EntityFrameworkCore.Storage;
using Vettingo.ExamService.Application.Repository;
using Vettingo.ExamService.Application.UnitOfWork;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Persistence.DbContext;
using Vettingo.ExamService.Persistence.Repository;

namespace Vettingo.ExamService.Persistence.UnitOfWork;

public sealed class ExamUnitOfWork(
    ExamDbContext context,
    ICapPublisher capPublisher,
    IDispatcher dispatcher) : IExamUnitOfWork
{
    private PostgreSqlCapTransaction? _transaction;

    public IExamRepository<JobStageExam> JobStageExamRepository { get; } = new ExamRepository<JobStageExam>(context);
    public IExamRepository<ExamAttempt> ExamAttemptRepository { get; } = new ExamRepository<ExamAttempt>(context);

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_transaction is not null || capPublisher.Transaction is not null)
        {
            throw new InvalidOperationException("Zaten aktif bir transaction var.");
        }

        var transaction = new PostgreSqlCapTransaction(dispatcher) { AutoCommit = false };
        _transaction = transaction;
        capPublisher.Transaction = transaction;
        return OpenTransactionAsync(transaction, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return context.SaveChangesAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        var transaction = _transaction ?? throw new InvalidOperationException("Aktif transaction bulunamadı.");
        await transaction.CommitAsync(cancellationToken);
        await DisposeTransactionAsync(transaction);
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction is not { } transaction)
        {
            return;
        }

        context.ChangeTracker.Clear();
        await DisposeTransactionAsync(transaction);
    }

    public ValueTask DisposeAsync() => new(RollbackTransactionAsync());

    private async Task OpenTransactionAsync(PostgreSqlCapTransaction transaction, CancellationToken cancellationToken)
    {
        try
        {
            transaction.DbTransaction = await context.Database.BeginTransactionAsync(cancellationToken);
        }
        catch
        {
            await DisposeTransactionAsync(transaction);
            throw;
        }
    }

    private ValueTask DisposeTransactionAsync(PostgreSqlCapTransaction transaction)
    {
        _transaction = null;
        capPublisher.Transaction = null;
        var databaseTransaction = (IDbContextTransaction?)transaction.DbTransaction;
        transaction.DbTransaction = null;
        transaction.Dispose();
        return databaseTransaction?.DisposeAsync() ?? ValueTask.CompletedTask;
    }
}
