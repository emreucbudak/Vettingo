using Vettingo.AnalyticsService.Application.Bases;

namespace Vettingo.AnalyticsService.Application.Exceptions
{
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}
