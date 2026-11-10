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
                var startingLimitTime = nextStep.StartingLimitTime!.Value;
                var stoppedLimitTime = nextStep.StoppedLimitTime!.Value;

                var dayCount = nextStep.EndDate.DayNumber - nextStep.StartedDate.DayNumber + 1;
                var dailyDuration = stoppedLimitTime.ToTimeSpan() - startingLimitTime.ToTimeSpan();
                var totalMinutes = dailyDuration.TotalMinutes * dayCount;
                var interviewCapacity = totalMinutes / nextStep.InterviewIntervalMinutes;

                if (interviewCapacity < passedApplications.Count)
                {
                    var remainingCandidateCount = passedApplications.Count - (int)interviewCapacity!.Value;
                    var interviewsPerDay = (int)(dailyDuration.TotalMinutes / nextStep.InterviewIntervalMinutes!.Value);
                    var additionalDays = (int)Math.Ceiling((double)remainingCandidateCount / interviewsPerDay);
                    nextStep.EndDate = nextStep.EndDate.AddDays(additionalDays);

                    var followingStep = jobPosting.JobSteps.FirstOrDefault(step => step.Order == nextStep.Order + 1);
                    if (followingStep is not null)
                    {
                        followingStep.StartedDate = followingStep.StartedDate.AddDays(additionalDays);
                        followingStep.EndDate = followingStep.EndDate.AddDays(additionalDays);
                    }
                }

                await candidateInterviewListPublisher.PublishAsync(new InterviewCreateMessage
                {
                    JobId = jobPosting.Id,
                    StepId = nextStep.Id,
                    CompanyId = jobPosting.CompanyId,
                    Title = jobPosting.Title,
                    StartedDate = nextStep.StartedDate,
                    EndDate = nextStep.EndDate,
                    StartingLimitTime = startingLimitTime,
                    StoppedLimitTime = stoppedLimitTime,
                    InterviewIntervalMinutes = nextStep.InterviewIntervalMinutes,
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
