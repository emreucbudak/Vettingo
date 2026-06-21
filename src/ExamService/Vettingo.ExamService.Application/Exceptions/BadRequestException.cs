using Vettingo.ExamService.Application.Bases;

namespace Vettingo.ExamService.Application.Exceptions
{
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
