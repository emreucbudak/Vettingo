using Vettingo.NotificationService.Application.Bases;

namespace Vettingo.NotificationService.Application.Exceptions
{
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
