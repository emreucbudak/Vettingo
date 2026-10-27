using Vettingo.AuthService.Application.DTOs;

namespace Vettingo.AuthService.Application.Messaging;

public interface ILoginCodeSenderPublisher
{
    Task CodeSend(LoginCodeMessage message, CancellationToken cancellationToken = default);
}
