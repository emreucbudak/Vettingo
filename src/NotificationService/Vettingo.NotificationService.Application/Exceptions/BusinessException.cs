using Vettingo.NotificationService.Application.Bases;

namespace Vettingo.NotificationService.Application.Exceptions
{
    public class BusinessException : BaseException
    {
        public BusinessException(string message) : base(message)
        {
        }
    }
}
