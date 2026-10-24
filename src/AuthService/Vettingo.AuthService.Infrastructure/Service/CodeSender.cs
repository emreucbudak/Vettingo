using DotNetCore.CAP;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Service;

namespace Vettingo.AuthService.Infrastructure.Service
{
    public class CodeSender : ICodeSender
    {
        private const string TopicName = "vettingo.auth.code.requested.v1";
        private readonly ICapPublisher _capPublisher;

        public CodeSender(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public async Task SendAsync(CodeMessage message, CancellationToken cancellationToken = default)
        {
            await _capPublisher.PublishAsync(TopicName, message, cancellationToken: cancellationToken);
        }
    }
}
