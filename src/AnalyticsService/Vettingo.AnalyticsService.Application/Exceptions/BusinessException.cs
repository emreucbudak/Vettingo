using Vettingo.AnalyticsService.Application.Bases;

namespace Vettingo.AnalyticsService.Application.Exceptions
{
    public class BusinessException : BaseException
    {
        public BusinessException(string message) : base(message)
        {
        }
    }
}
