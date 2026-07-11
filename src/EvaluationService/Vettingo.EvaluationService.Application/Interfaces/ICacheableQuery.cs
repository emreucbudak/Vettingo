namespace Vettingo.EvaluationService.Application.Interfaces;

public interface ICacheableQuery
{
    string CacheKey { get; set; }
    TimeSpan ExpirationTime { get; set; }
}
