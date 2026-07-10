namespace Vettingo.InterviewService.Application.Interfaces
{
    public interface ICacheableQuery
    {
        public string CacheKey { get; set; }
        public TimeSpan ExpirationTime { get; set; }
    }
}
