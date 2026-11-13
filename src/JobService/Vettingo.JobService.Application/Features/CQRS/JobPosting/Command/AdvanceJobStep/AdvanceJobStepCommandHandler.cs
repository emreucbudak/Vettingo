using FlashMediator;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Messaging;
using Vettingo.JobService.Application.UnitOfWork;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.AdvanceJobStep;

public class AdvanceJobStepCommandHandler(
    IJobUnitOfWork unitOfWork,
    ICandidateExamListPublisher candidateExamListPublisher,
    ICandidateInterviewListPublisher candidateInterviewListPublisher,
    IInterviewStageExtendedPublisher interviewStageExtendedPublisher,
    IExamStageAdvancedPublisher examStageAdvancedPublisher,
    IInterviewStageAdvancedPublisher interviewStageAdvancedPublisher,
    IOfferStageAdvancedPublisher offerStageAdvancedPublisher) : IRequestHandler<AdvanceJobStepCommandRequest>
{
    public async Task Handle(AdvanceJobStepCommandRequest request, CancellationToken cancellationToken)
    {
        var expiredJobPostings = await unitOfWork.JobPostingRepository.GetExpiredJobPostingsAsync(cancellationToken);

        foreach (var jobPosting in expiredJobPostings)
        {
            var passedApplications = await unitOfWork.JobApplicationRepository.GetPassedApplicationsByJobPostingIdAsync(
                jobPosting.Id, cancellationToken);

            if (jobPosting.GetCurrentStepType() == StepType.CandidateCollection)
            {
                jobPosting.CloseJobPostingApplications();
            }

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

                if (passedApplications.Count > 0)
                {
                    await examStageAdvancedPublisher.PublishAsync(new ExamStageAdvancedRequestedMessage
                    {
                        NotificationIdsByCandidateId = passedApplications.Select(application => application.CandidateId).Distinct().ToDictionary(id => id, _ => Guid.CreateVersion7()),
                        CompanyId = jobPosting.CompanyId,
                        Title = jobPosting.Title,
                        StartedDate = nextStep.StartedDate,
                        EndDate = nextStep.EndDate
                    }, cancellationToken);
                }
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

                    var lastShiftedOrder = nextStep.Order;
                    while (jobPosting.JobSteps.FirstOrDefault(step => step.Order == lastShiftedOrder + 1) is { } followingStep)
                    {
                        followingStep.StartedDate = followingStep.StartedDate.AddDays(additionalDays);
                        followingStep.EndDate = followingStep.EndDate.AddDays(additionalDays);
                        lastShiftedOrder++;
                    }

                    if (lastShiftedOrder > nextStep.Order)
                    {
                        foreach (var application in passedApplications)
                        {
                            await interviewStageExtendedPublisher.SendExtendedNotification(new InterviewStageExtendedMessage
                            {
                                CandidateId = application.CandidateId,
                                CompanyId = jobPosting.CompanyId,
                                Title = jobPosting.Title,
                                AdditionalDays = additionalDays,
                                EndDate = nextStep.EndDate
                            }, cancellationToken);
                        }
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

                if (passedApplications.Count > 0)
                {
                    await interviewStageAdvancedPublisher.PublishAsync(new InterviewStageAdvancedRequestedMessage
                    {
                        NotificationIdsByCandidateId = passedApplications.Select(application => application.CandidateId).Distinct().ToDictionary(id => id, _ => Guid.CreateVersion7()),
                        CompanyId = jobPosting.CompanyId,
                        Title = jobPosting.Title,
                        StartedDate = nextStep.StartedDate,
                        EndDate = nextStep.EndDate,
                        InterviewChapterName = nextStep.InterviewChapterName
                    }, cancellationToken);
                }
            }
            else if (nextStep?.StepType == StepType.Offer)
            {
                var offers = new Offers();
                offers.CreateOffers(
                    jobPosting.Id,
                    nextStep.StartedDate,
                    nextStep.EndDate,
                    passedApplications.Select(application => application.CandidateId).ToList());
                await unitOfWork.OffersRepository.AddAsync(offers, cancellationToken);

                if (offers.CandidateIds.Count > 0)
                {
                    await offerStageAdvancedPublisher.PublishAsync(new OfferStageAdvancedRequestedMessage
                    {
                        NotificationIdsByCandidateId = offers.CandidateIds.Distinct()
                            .ToDictionary(candidateId => candidateId, _ => Guid.CreateVersion7()),
                        CompanyId = jobPosting.CompanyId,
                        Title = jobPosting.Title,
                        StartedDate = nextStep.StartedDate,
                        EndDate = nextStep.EndDate
                    }, cancellationToken);
                }
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

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
