using Vettingo.ExamService.Application.Bases;

namespace Vettingo.ExamService.Application.Exceptions
{
    public class BusinessException : BaseException
    {
        public BusinessException(string message) : base(message)
        {
        }
    }
}
