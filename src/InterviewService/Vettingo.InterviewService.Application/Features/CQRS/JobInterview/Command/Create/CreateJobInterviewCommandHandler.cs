using FlashMediator;
using InterviewEntity = Vettingo.InterviewService.Domain.Entities.Interview;
using JobInterviewEntity = Vettingo.InterviewService.Domain.Entities.JobInterview;

namespace Vettingo.InterviewService.Application.Features.CQRS.JobInterview.Command.Create;

public class CreateJobInterviewCommandHandler : IRequestHandler<CreateJobInterviewCommandRequest>
{
    public Task Handle(CreateJobInterviewCommandRequest request, CancellationToken cancellationToken)
    {
        var jobInterview = new JobInterviewEntity();
        jobInterview.CreateJobInterview(
            request.StepId,
            request.JobId,
            request.StartedDate,
            request.EndDate,
            request.WindowStartTime,
            request.WindowEndTime,
            request.CandidatesToInterview);

        var interviews = new List<InterviewEntity>();
        foreach (var candidateId in request.CandidatesToInterview)
        {
            var interview = new InterviewEntity();
            interviews.Add(interview);
        }

        jobInterview.SetInterviews(interviews);

        return Task.CompletedTask;
    }
}
