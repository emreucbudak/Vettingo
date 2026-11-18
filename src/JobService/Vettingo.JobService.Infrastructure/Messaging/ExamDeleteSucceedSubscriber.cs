using DotNetCore.CAP;
using Vettingo.JobService.Infrastructure.Messages;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class ExamDeleteSucceedSubscriber : ICapSubscribe
{
    [CapSubscribe("vettingo.job.exam.delete.succeeded.v1", Group = "vettingo.job-service")]
    public Task HandleAsync(ExamDeleteSucceedMessage message, CancellationToken cancellationToken)
        => throw new NotImplementedException("Sınav silme başarı mesajının işleyicisi henüz bağlanmadı.");
}
