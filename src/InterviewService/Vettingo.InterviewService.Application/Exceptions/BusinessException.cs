using Vettingo.InterviewService.Application.Bases;

namespace Vettingo.InterviewService.Application.Exceptions
{
    public class BusinessException : BaseException
    {
        public BusinessException(string message) : base(message)
        {
        }
    }
}
