using DotNetCore.CAP;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Messaging;

namespace Vettingo.AuthService.Infrastructure.Messaging
{
    public class CompanyStaticsCreatePublisher : ICompanyStaticsCreatePublisher
    {
        private readonly ICapPublisher _capPublisher;

        public CompanyStaticsCreatePublisher(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public Task SendInfo(CreateCompanyStaticsMessage message)
        {
            return _capPublisher.PublishAsync(CreateCompanyStaticsMessage.TopicName, message);
        }
    }
}
