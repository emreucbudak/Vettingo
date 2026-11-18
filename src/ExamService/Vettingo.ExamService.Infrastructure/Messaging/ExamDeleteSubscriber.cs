using DotNetCore.CAP;
using Vettingo.ExamService.Infrastructure.Messages;

namespace Vettingo.ExamService.Infrastructure.Messaging;

public sealed class ExamDeleteSubscriber : ICapSubscribe
{
    [CapSubscribe("vettingo.exam.exam.delete.requested.v1", Group = "vettingo.exam-service")]
    public Task HandleAsync(ExamDeleteMessage message, CancellationToken cancellationToken)
        => throw new NotImplementedException("Sınav silme mesajının işleyicisi henüz bağlanmadı.");
}
