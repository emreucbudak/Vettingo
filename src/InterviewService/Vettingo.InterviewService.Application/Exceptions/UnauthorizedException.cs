using Vettingo.InterviewService.Application.Bases;

namespace Vettingo.InterviewService.Application.Exceptions
{
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}
