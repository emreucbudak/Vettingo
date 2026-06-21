using Vettingo.ExamService.Application.Bases;

namespace Vettingo.ExamService.Application.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
