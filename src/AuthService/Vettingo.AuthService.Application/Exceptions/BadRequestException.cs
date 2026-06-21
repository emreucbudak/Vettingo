using Vettingo.AuthService.Application.Bases;

namespace Vettingo.AuthService.Application.Exceptions
{
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
