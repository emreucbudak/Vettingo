using Vettingo.AuthService.Application.DTOs;

namespace Vettingo.AuthService.Application.Messaging
{
    public interface ICompanyStaticsCreatePublisher
    {
        Task SendInfo(CreateCompanyStaticsMessage message);
    }
}
