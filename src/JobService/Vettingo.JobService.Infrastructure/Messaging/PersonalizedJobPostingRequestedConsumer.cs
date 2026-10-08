using DotNetCore.CAP;
using FlashMediator;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.CreatePersonalizedJobPosting;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class PersonalizedJobPostingRequestedConsumer(
    IMediator mediator,
    IValidator<CreatePersonalizedJobPostingCommandRequest> validator,
    ILogger<PersonalizedJobPostingRequestedConsumer> logger) : ICapSubscribe
{
    [CapSubscribe(PersonalizedJobPostingMessage.TopicName, Group = "vettingo.job-service")]
    public async Task HandleAsync(
        PersonalizedJobPostingMessage message,
        CancellationToken cancellationToken)
    {
        var request = new CreatePersonalizedJobPostingCommandRequest
        {
            UserId = message.UserId,
            Title = message.Title,
            PublishedDate = message.PublishedDate,
            CityId = message.CityId,
            Salary = message.Salary
        };

        await validator.ValidateAndThrowAsync(request, cancellationToken);
        logger.LogInformation("Personalized ilan eventi işleniyor. UserId: {UserId}", message.UserId);
        await mediator.Send(request, cancellationToken);
    }
}
