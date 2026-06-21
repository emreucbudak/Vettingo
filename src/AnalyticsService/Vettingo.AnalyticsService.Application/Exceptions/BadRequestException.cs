using Vettingo.AnalyticsService.Application.Bases;

namespace Vettingo.AnalyticsService.Application.Exceptions
{
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
