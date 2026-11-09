using FlashMediator;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.AdvanceJobStep;

public class AdvanceJobStepCommandHandler(IJobPostingRepository jobPostingRepository) : IRequestHandler<AdvanceJobStepCommandRequest>
{
    public async Task Handle(AdvanceJobStepCommandRequest request, CancellationToken cancellationToken)
    {
        var expiredJobPostings = await jobPostingRepository.GetExpiredJobPostingsAsync(cancellationToken);
    }
}
