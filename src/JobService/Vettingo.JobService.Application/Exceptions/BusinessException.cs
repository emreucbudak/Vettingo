using Vettingo.JobService.Application.Bases;

namespace Vettingo.JobService.Application.Exceptions
{
    public class BusinessException : BaseException
    {
        public BusinessException(string message) : base(message)
        {
        }
    }
}
