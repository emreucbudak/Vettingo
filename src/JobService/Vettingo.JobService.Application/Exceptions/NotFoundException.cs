using Vettingo.JobService.Application.Bases;

namespace Vettingo.JobService.Application.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
