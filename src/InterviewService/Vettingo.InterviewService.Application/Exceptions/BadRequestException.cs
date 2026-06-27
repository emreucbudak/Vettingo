using Vettingo.InterviewService.Application.Bases;

namespace Vettingo.InterviewService.Application.Exceptions
{
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
