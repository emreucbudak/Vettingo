using Vettingo.InterviewService.Application.Bases;

namespace Vettingo.InterviewService.Application.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
