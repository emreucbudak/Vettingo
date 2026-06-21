using Vettingo.NotificationService.Application.Bases;

namespace Vettingo.NotificationService.Application.Exceptions
{
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}
