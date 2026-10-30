using FlashMediator;
using Vettingo.InterviewService.Application.Repository;

namespace Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetByDate;

public sealed class GetInterviewsByDateQueryHandler(IInterviewRepository repository)
    : IRequestHandler<GetInterviewsByDateQueryRequest, IEnumerable<GetInterviewsByDateQueryResponse>>
{
    public async Task<IEnumerable<GetInterviewsByDateQueryResponse>> Handle(
        GetInterviewsByDateQueryRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var interviews = await repository.GetByCompanyAndDateAsync(request.CompanyId, request.Date, cancellationToken);

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
