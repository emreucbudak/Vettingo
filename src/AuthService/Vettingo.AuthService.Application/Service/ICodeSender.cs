using Vettingo.AuthService.Application.DTOs;

namespace Vettingo.AuthService.Application.Service
{
    public interface ICodeSender
    {
        Task SendAsync(CodeMessage message, CancellationToken cancellationToken = default);
    }
}
