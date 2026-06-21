using Vettingo.AnalyticsService.Application.Bases;

namespace Vettingo.AnalyticsService.Application.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
