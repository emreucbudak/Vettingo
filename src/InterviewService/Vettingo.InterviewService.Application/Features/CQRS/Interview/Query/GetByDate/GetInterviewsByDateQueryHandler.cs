using FlashMediator;
using Vettingo.InterviewService.Application.Repository;
using InterviewEntity = Vettingo.InterviewService.Domain.Entities.Interview;

namespace Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetByDate;

public sealed class GetInterviewsByDateQueryHandler(IRepository<InterviewEntity> repository)
    : IRequestHandler<GetInterviewsByDateQueryRequest, IEnumerable<GetInterviewsByDateQueryResponse>>
{
    public async Task<IEnumerable<GetInterviewsByDateQueryResponse>> Handle(
        GetInterviewsByDateQueryRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var interviews = await repository.GetAllAsync(
            interview => interview.CompanyId == request.CompanyId && interview.InterviewDate == request.Date,
            query => query.OrderBy(interview => interview.StartedTime).ThenBy(interview => interview.Id));

        return interviews.Select(interview => new GetInterviewsByDateQueryResponse
        {
            Id = interview.Id,
            UserId = interview.UserId,
            InterviewDate = interview.InterviewDate,
            StartedTime = interview.StartedTime,
            Name = interview.Name,
            Surname = interview.Surname,
            Chapter = interview.Chapter,
            Role = interview.Role,
            WhereIsMeeting = interview.WhereIsMeeting,
            MeetingLink = interview.MeetingLink
        }).ToArray();
    }
}
