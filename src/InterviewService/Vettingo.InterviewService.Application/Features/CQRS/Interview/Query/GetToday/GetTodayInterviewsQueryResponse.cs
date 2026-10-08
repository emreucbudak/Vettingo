namespace Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetToday
{
    public record GetTodayInterviewsQueryResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Surname { get; init; } = string.Empty;
        public TimeOnly StartedTime { get; init; }
    }
}
