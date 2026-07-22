using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.ApplicationService.Application.Exceptions;
using Vettingo.ApplicationService.Application.Repository;

namespace Vettingo.ApplicationService.Application.Features.CQRS.JobApplication.Command.UpdateStatus
{
    public class UpdateJobApplicationStatusCommandHandler(
        IJobApplicationRepository repository,
        ILogger<UpdateJobApplicationStatusCommandHandler> logger)
        : IRequestHandler<UpdateJobApplicationStatusCommandRequest>
    {
        public async Task Handle(UpdateJobApplicationStatusCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(UpdateJobApplicationStatusCommandHandler));

            var application = await repository.GetByIdAsync(request.ApplicationId)
                ?? throw new NotFoundException("Başvuru bulunamadı.");

            application.UpdateStatus(request.Status);
            repository.Update(application);
            await repository.SaveChangesAsync();
        }
    }
}
