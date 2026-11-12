using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Messaging;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CreateJobPosting
{
    public class CreateJobPostingCommandHandler(
        IJobPostingRepository jobPostingRepository,
        ILogger<CreateJobPostingCommandHandler> logger,
        IJobAnalyticsPublisher jobAnalyticsPublisher) : IRequestHandler<CreateJobPostingCommandRequest>
    {
        private readonly IJobAnalyticsPublisher _jobAnalyticsPublisher = jobAnalyticsPublisher;

        public async Task Handle(CreateJobPostingCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateJobPostingCommandHandler));
            ArgumentNullException.ThrowIfNull(request.JobSteps, nameof(request.JobSteps));
            if (request.JobSteps.Any(step => step is null))
            {
                throw new ArgumentException("İş ilanı adımları boş olamaz.", nameof(request.JobSteps));
            }

            List<Domain.Entities.JobSteps> jobSteps = request.JobSteps.Select(step => new Domain.Entities.JobSteps
            {
                Order = step.Order,
                StepType = step.StepType,
                ExamId = step.ExamId,
                StartedDate = step.StartedDate,
                EndDate = step.EndDate,
                StartingLimitTime = step.StartingLimitTime,
                StoppedLimitTime = step.StoppedLimitTime,
                InterviewIntervalMinutes = step.InterviewIntervalMinutes,
                InterviewChapterName = step.InterviewChapterName,
                MeetingLink = step.MeetingLink
            }).ToList();

            Domain.Entities.JobPosting jobPosting = new();
            jobPosting.CreateJobPosting(
                request.CompanyId,
                request.Title,
                request.CityId,
                request.EmploymentType,
                request.WorkingModel,
                request.MinimumExperienceYear,
                request.MaximumExperienceYear,
                request.MinSalary,
                request.MaxSalary,
                jobSteps);

            await jobPostingRepository.AddJobPostingAsync(jobPosting);
            await jobPostingRepository.SaveChangesAsync();
        }
    }
}


