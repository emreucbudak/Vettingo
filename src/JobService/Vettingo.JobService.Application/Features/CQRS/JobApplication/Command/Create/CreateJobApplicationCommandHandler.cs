using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.Repository;
using JobApplicationEntity = Vettingo.JobService.Domain.Entities.JobApplication;

namespace Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.Create
{
    public class CreateJobApplicationCommandHandler(
        IJobApplicationRepository repository,
        IJobPostingRepository jobPostings,
        ILogger<CreateJobApplicationCommandHandler> logger)
        : IRequestHandler<CreateJobApplicationCommandRequest, CreateJobApplicationCommandResponse>
    {
        public async Task<CreateJobApplicationCommandResponse> Handle(
            CreateJobApplicationCommandRequest request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateJobApplicationCommandHandler));

            if (await repository.ExistsAsync(request.CandidateId, request.JobPostingId))
            {
                throw new BadRequestException("Aday bu iş ilanına daha önce başvurdu.");
            }

            var posting = await jobPostings.GetJobPostingByIdAsync(request.JobPostingId)
                ?? throw new NotFoundException("İş ilanı bulunamadı.");
            if (posting.Status != Domain.Enums.JobPostingStatus.Active)
                throw new BadRequestException("Bu ilan başvuruya açık değil.");

            JobApplicationEntity application = new();
            application.CreateApplication(
                request.CandidateId,
                request.JobPostingId,
                request.AppliedAt ?? DateTime.UtcNow,
                request.Status);

            try
            {
                await repository.AddAsync(application);
                await repository.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Başvuru kaydedilemedi. JobPostingId: {JobPostingId}, CandidateId: {CandidateId}", application.JobPostingId, application.CandidateId);
                throw;
            }


            return new CreateJobApplicationCommandResponse(application.Id);
        }
    }
}
