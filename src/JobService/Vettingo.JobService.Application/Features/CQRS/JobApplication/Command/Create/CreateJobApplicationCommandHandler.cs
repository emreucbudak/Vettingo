using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.Messaging;
using Vettingo.JobService.Application.UnitOfWork;
using Vettingo.JobService.Domain.Enums;
using JobApplicationEntity = Vettingo.JobService.Domain.Entities.JobApplication;

namespace Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.Create
{
    public class CreateJobApplicationCommandHandler(
        IJobUnitOfWork unitOfWork,
        IJobApplicationCreatedPublisher publisher,
        ILogger<CreateJobApplicationCommandHandler> logger)
        : IRequestHandler<CreateJobApplicationCommandRequest>
    {
        public async Task Handle(
            CreateJobApplicationCommandRequest request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateJobApplicationCommandHandler));

            var repository = unitOfWork.JobApplicationRepository;
            if (await repository.ExistsAsync(request.CandidateId, request.JobPostingId))
            {
                throw new BadRequestException("Aday bu iş ilanına daha önce başvurdu.");
            }

            var posting = await unitOfWork.JobPostingRepository.GetJobPostingByIdAsync(request.JobPostingId)
                ?? throw new NotFoundException("İş ilanı bulunamadı.");
            if (posting.IsPostingClosed || posting.IsApplicationClosed || posting.GetCurrentStepType() != StepType.CandidateCollection)
                throw new BadRequestException("Bu ilan başvuruya açık değil.");

            JobApplicationEntity application = new();
            application.CreateApplication(
                request.CandidateId,
                request.JobPostingId,
                request.AppliedAt ?? DateTime.UtcNow,
                request.Status);
            application.MarkCurrentStepPassed();

            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await repository.AddAsync(application);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await publisher.PublishAsync(new JobApplicationCreatedMessage
                {
                    Id = application.Id,
                    UserId = application.CandidateId
                }, cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                logger.LogError(exception, "Başvuru kaydedilemedi. JobPostingId: {JobPostingId}, CandidateId: {CandidateId}", application.JobPostingId, application.CandidateId);
                throw;
            }
        }
    }
}
