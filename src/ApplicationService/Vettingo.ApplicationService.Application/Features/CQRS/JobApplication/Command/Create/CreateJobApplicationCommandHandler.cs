using FlashMediator;
using Vettingo.ApplicationService.Application.Messaging;
using Microsoft.Extensions.Logging;
using Vettingo.ApplicationService.Application.Exceptions;
using Vettingo.ApplicationService.Application.Repository;
using JobApplicationEntity = Vettingo.ApplicationService.Domain.Entities.JobApplication;

namespace Vettingo.ApplicationService.Application.Features.CQRS.JobApplication.Command.Create
{
    public class CreateJobApplicationCommandHandler(
        IJobApplicationRepository repository,
        IJobApplicationCreatedPublisher publisher,
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

            try
            {
                await publisher.PublishAsync(application.JobPostingId, cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Başvuru kaydedildi ancak ilan sayacı mesajı gönderilemedi. JobPostingId: {JobPostingId}", application.JobPostingId);
            }

            return new CreateJobApplicationCommandResponse(application.Id);
        }
    }
}
