using Vettingo.AuthService.Application.DTOs;

namespace Vettingo.AuthService.Application.Messaging;

public interface IResetPasswordCodeSenderPublisher
{
    Task CodeSend(CodeMessage message, CancellationToken cancellationToken = default);
}
