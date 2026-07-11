namespace Vettingo.InterviewService.Application.Interfaces
{
    public interface ICacheableQuery
    {
        public string CacheKey { get; }
        public TimeSpan ExpirationTime { get; }
    }
}
