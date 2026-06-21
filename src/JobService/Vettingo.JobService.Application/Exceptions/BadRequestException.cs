using Vettingo.JobService.Application.Bases;

namespace Vettingo.JobService.Application.Exceptions
{
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
