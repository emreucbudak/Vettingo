using FlashMediator;
using Vettingo.JobService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.UpdateJobPosting
{
    public class UpdateJobPostingCommandHandler(IJobPostingRepository jobPostingRepository, ILogger<UpdateJobPostingCommandHandler> logger) : IRequestHandler<UpdateJobPostingCommandRequest>
    {
        public async Task Handle(UpdateJobPostingCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(UpdateJobPostingCommandHandler));
            if (request.JobSteps is not { Count: > 0 } || request.JobSteps.Any(step => step is null))
            {
                throw new BadRequestException("İş ilanı için en az bir geçerli adım gereklidir.");
            }

            var jobPosting = await jobPostingRepository.GetJobPostingByIdAsync(request.JobPostingId);

            if (jobPosting is null)
            {
                throw new NotFoundException("İş ilanı bulunamadı");
            }

            jobPosting.UpdateJobPosting(
                request.Title,
                request.CityId,
                request.EmploymentType,
                request.WorkingModel,
                request.MinimumExperienceYear,
                request.MaximumExperienceYear,
                request.MinSalary,
                request.MaxSalary);

            jobPostingRepository.UpdateJobPosting(jobPosting);
            await jobPostingRepository.SaveChangesAsync();
        }
    }
}



