using Vettingo.AuthService.Application.Bases;

namespace Vettingo.AuthService.Application.Exceptions
{
    public class BusinessException : BaseException
    {
        public BusinessException(string message) : base(message)
        {
        }
    }
}
