using FlashMediator;
using Vettingo.InterviewService.Application.Repository;
using InterviewEntity = Vettingo.InterviewService.Domain.Entities.Interview;

namespace Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetToday
{
    public class GetTodayInterviewsQueryHandler(IRepository<InterviewEntity> repository, TimeProvider timeProvider)
        : IRequestHandler<GetTodayInterviewsQueryRequest, IEnumerable<GetTodayInterviewsQueryResponse>>
    {
        private static readonly TimeZoneInfo InterviewTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");

        public async Task<IEnumerable<GetTodayInterviewsQueryResponse>> Handle(
            GetTodayInterviewsQueryRequest request, CancellationToken cancellationToken)
        {
            DateOnly today = DateOnly.FromDateTime(
                TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), InterviewTimeZone).DateTime);

            var interviews = await repository.GetAllAsync(
                interview => interview.CompanyId == request.CompanyId && interview.InterviewDate == today,
                query => query.OrderBy(interview => interview.StartedTime).ThenBy(interview => interview.Id));

            return interviews.Select(interview => new GetTodayInterviewsQueryResponse
            {
                Id = interview.Id,
                Name = interview.Name,
                Surname = interview.Surname,
                StartedTime = interview.StartedTime
            }).ToArray();
        }
    }
}
