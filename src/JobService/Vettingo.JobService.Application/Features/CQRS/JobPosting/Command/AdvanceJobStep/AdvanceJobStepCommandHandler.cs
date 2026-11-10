using FlashMediator;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Messaging;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.AdvanceJobStep;

public class AdvanceJobStepCommandHandler(
    IJobPostingRepository jobPostingRepository,
    IJobApplicationRepository jobApplicationRepository,
    ICandidateExamListPublisher candidateExamListPublisher,
    ICandidateInterviewListPublisher candidateInterviewListPublisher) : IRequestHandler<AdvanceJobStepCommandRequest>
{
    public async Task Handle(AdvanceJobStepCommandRequest request, CancellationToken cancellationToken)
    {
        var expiredJobPostings = await jobPostingRepository.GetExpiredJobPostingsAsync(cancellationToken);

        foreach (var jobPosting in expiredJobPostings)
        {
            var passedApplications = await jobApplicationRepository.GetPassedApplicationsByJobPostingIdAsync(
                jobPosting.Id, cancellationToken);

            jobPosting.AdvanceCurrentStep();

            var nextStep = jobPosting.JobSteps.FirstOrDefault(step => step.Order == jobPosting.CurrentStep);
            if (nextStep?.StepType == StepType.Exam)
            {
                await candidateExamListPublisher.PublishAsync(new ContinuingCandidateListMessage
                {
                    JobId = jobPosting.Id,
                    StepId = nextStep.Id,
                    ExamId = nextStep.ExamId!.Value,
                    StartedDate = nextStep.StartedDate,
                    EndDate = nextStep.EndDate,
                    CandidateIds = passedApplications.Select(application => application.CandidateId).ToList()
                }, cancellationToken);
            }
            else if (nextStep?.StepType == StepType.Interview)
            {
                await candidateInterviewListPublisher.PublishAsync(new InterviewCreateMessage
                {
                    JobId = jobPosting.Id,
                    CompanyId = jobPosting.CompanyId,
                    Title = jobPosting.Title,
                    StartedDate = nextStep.StartedDate,
                    EndDate = nextStep.EndDate,
                    StartingLimitTime = nextStep.StartingLimitTime,
                    StoppedLimitTime = nextStep.StoppedLimitTime,
                    InterviewChapterName = nextStep.InterviewChapterName,
                    MeetingLink = nextStep.MeetingLink,
                    CandidateIds = passedApplications.Select(application => application.CandidateId).ToList()
                }, cancellationToken);
            }
            else if (nextStep is null)
            {
                jobPosting.CloseJobPosting();
                continue;
            }
            passedApplications.ForEach(application =>
            {
                application.ResetCurrentStepPassed();
            });
        }
    }
}
