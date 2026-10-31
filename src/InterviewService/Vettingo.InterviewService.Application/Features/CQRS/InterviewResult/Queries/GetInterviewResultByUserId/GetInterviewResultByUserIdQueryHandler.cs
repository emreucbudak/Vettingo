using FlashMediator;
using Vettingo.InterviewService.Application.Exceptions;
using Vettingo.InterviewService.Application.Repository;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Queries.GetInterviewResultByUserId;

public class GetInterviewResultByUserIdQueryHandler(
    IInterviewResultRepository repository, IInterviewRepository interviewRepository)
    : IRequestHandler<GetInterviewResultByUserIdQueryRequest, IEnumerable<GetInterviewResultByUserIdQueryResponse>>
{
    public async Task<IEnumerable<GetInterviewResultByUserIdQueryResponse>> Handle(
        GetInterviewResultByUserIdQueryRequest request, CancellationToken cancellationToken)
    {
        var results = await repository.GetByUserIdAsync(request.UserId, cancellationToken);

        var response = new List<GetInterviewResultByUserIdQueryResponse>();
        foreach (var result in results)
        {
            var interview = await interviewRepository.GetByIdAsync(result.InterviewId, cancellationToken)
                ?? throw new NotFoundException("Mülakat bulunamadı.");

            response.Add(new GetInterviewResultByUserIdQueryResponse
            {
                Chapter = interview.Chapter,
                Role = interview.Role,
                InterviewDate = interview.InterviewDate,
                InterviewScore = result.InterviewScore,
                Passed = result.Passed
            });
        }

        return response;
    }
}
