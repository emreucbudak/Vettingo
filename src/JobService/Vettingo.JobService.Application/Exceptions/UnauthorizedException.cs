using Vettingo.JobService.Application.Bases;

namespace Vettingo.JobService.Application.Exceptions
{
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}
