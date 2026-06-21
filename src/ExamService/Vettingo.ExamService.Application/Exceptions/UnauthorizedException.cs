using Vettingo.ExamService.Application.Bases;

namespace Vettingo.ExamService.Application.Exceptions
{
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}
