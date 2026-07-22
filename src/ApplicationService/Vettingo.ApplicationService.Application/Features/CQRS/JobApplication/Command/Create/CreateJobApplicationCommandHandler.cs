using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.ApplicationService.Application.Exceptions;
using Vettingo.ApplicationService.Application.Repository;
using JobApplicationEntity = Vettingo.ApplicationService.Domain.Entities.JobApplication;

namespace Vettingo.ApplicationService.Application.Features.CQRS.JobApplication.Command.Create
{
    public class CreateJobApplicationCommandHandler(
        IJobApplicationRepository repository,
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

            await repository.AddAsync(application);
            await repository.SaveChangesAsync();
            return new CreateJobApplicationCommandResponse(application.Id);
        }
    }
}
