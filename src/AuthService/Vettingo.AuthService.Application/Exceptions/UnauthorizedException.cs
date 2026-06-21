using Vettingo.AuthService.Application.Bases;

namespace Vettingo.AuthService.Application.Exceptions
{
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}
