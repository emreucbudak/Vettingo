using Vettingo.AuthService.Application.DTOs;

namespace Vettingo.AuthService.Application.Messaging;

public interface IRegisterConfirmCodeSenderPublisher
{
    Task CodeSend(CodeMessage message, CancellationToken cancellationToken = default);
}
