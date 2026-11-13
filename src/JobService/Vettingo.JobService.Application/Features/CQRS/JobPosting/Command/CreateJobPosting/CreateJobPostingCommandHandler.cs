using FlashMediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Messaging;
using Vettingo.JobService.Application.UnitOfWork;
using Vettingo.JobService.Domain.Entities;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CreateJobPosting
{
    public class CreateJobPostingCommandHandler(
        IJobUnitOfWork unitOfWork,
        ILogger<CreateJobPostingCommandHandler> logger,
        IJobAnalyticsPublisher jobAnalyticsPublisher,
        IJobPostingPublishedPublisher jobPostingPublishedPublisher) : IRequestHandler<CreateJobPostingCommandRequest>
    {
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

            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var skillTitles = request.CandidateSkills.Select(skill => skill.SkillTitle).ToArray();
                var skillsByTitle = await unitOfWork.Skills
                    .AsTracking()
                    .Where(skill => skillTitles.Contains(skill.Title))
                    .ToDictionaryAsync(skill => skill.Title, cancellationToken);

                foreach (var requestedSkill in request.CandidateSkills)
                {
                    if (!skillsByTitle.TryGetValue(requestedSkill.SkillTitle, out var skill))
                    {
                        skill = new Skill();
                        skill.CreateSkill(requestedSkill.SkillTitle);
                        skillsByTitle.Add(skill.Title, skill);
                    }

                    jobPosting.AddCandidateSkill(skill, requestedSkill.ExperienceYear);
                }

                await unitOfWork.JobPostingRepository.AddJobPostingAsync(jobPosting);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                await jobAnalyticsPublisher.PublishAsync(new JobAnalyticsDto
                {
                    CompanyId = jobPosting.CompanyId,
                    EventId = jobPosting.Id
                }, cancellationToken);
                await jobPostingPublishedPublisher.PublishAsync(new JobPostingPublishedMessage
                {
                    JobPostingId = jobPosting.Id,
                    Title = jobPosting.Title,
                    CityId = jobPosting.CityId,
                    EmploymentType = jobPosting.EmploymentType,
                    WorkingModel = jobPosting.WorkingModel,
                    MinimumExperienceYear = jobPosting.MinimumExperienceYear,
                    MaximumExperienceYear = jobPosting.MaximumExperienceYear,
                    MinSalary = jobPosting.MinSalary,
                    MaxSalary = jobPosting.MaxSalary,
                    CandidateSkills = jobPosting.CandidateSkills
                        .Select(skill => new StepMessageDto(skill.Skill.Title, skill.ExperienceYear))
                        .ToList()
                }, cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }
    }
}


