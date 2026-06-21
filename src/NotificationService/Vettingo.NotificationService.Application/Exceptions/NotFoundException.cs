using Vettingo.NotificationService.Application.Bases;

namespace Vettingo.NotificationService.Application.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
