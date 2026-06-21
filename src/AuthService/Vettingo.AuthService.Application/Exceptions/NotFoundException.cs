using Vettingo.AuthService.Application.Bases;

namespace Vettingo.AuthService.Application.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
