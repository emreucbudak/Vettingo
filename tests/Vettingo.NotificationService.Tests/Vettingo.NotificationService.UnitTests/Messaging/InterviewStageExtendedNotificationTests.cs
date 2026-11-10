using System.Reflection;
using System.Text.Json;
using DotNetCore.CAP;
using FlashMediator;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Query.SendInterviewStageExtendedNotification;
using Vettingo.AuthService.Application.Messaging;
using Vettingo.AuthService.Application.Repository;
using Vettingo.AuthService.Domain.Entities;
using Vettingo.AuthService.Infrastructure.Messages;
using Vettingo.NotificationService.Application.DTOs;
using Vettingo.NotificationService.Application.Services;
using Vettingo.NotificationService.Infrastructure.Messaging;
using Vettingo.NotificationService.Infrastructure.Messages;
using AuthPublisher = Vettingo.AuthService.Infrastructure.Messaging.InterviewStageExtendedPublisher;
using AuthSubscriber = Vettingo.AuthService.Infrastructure.Messaging.InterviewStageExtendedSubscriber;

namespace Vettingo.NotificationService.UnitTests.Messaging;

public sealed class InterviewStageExtendedNotificationTests
{
    [Fact]
    public async Task Auth_Query_Should_Resolve_Recipient_And_Company_And_Notification_Should_Send_Html_Email()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "candidate@example.com" };
        var store = Substitute.For<IUserStore<User>>();
        store.FindByIdAsync(user.Id.ToString("D"), Arg.Any<CancellationToken>()).Returns(user);
        using var userManager = new UserManager<User>(store, Options.Create(new IdentityOptions()),
            new PasswordHasher<User>(), [], [], new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), null!, NullLogger<UserManager<User>>.Instance);

        var company = new Company();
        company.RegisterCompany("Vettingo & Partners");
        var companyRepository = Substitute.For<ICompanyRepository>();
        companyRepository.GetCompanyByIdAsync(company.Id).Returns(company);
        var capPublisher = Substitute.For<ICapPublisher>();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(userManager);
        services.AddSingleton(companyRepository);
        services.AddSingleton(capPublisher);
        services.AddScoped<IInterviewStageExtendedPublisher, AuthPublisher>();
        services.AddFlashMediator(typeof(SendInterviewStageExtendedNotificationQueryHandler).Assembly);
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var authSubscriber = new AuthSubscriber(scope.ServiceProvider.GetRequiredService<IMediator>());
        using var cancellation = new CancellationTokenSource();

        await authSubscriber.HandleAsync(new InterviewStageExtendedRequestedMessage
        {
            CandidateId = user.Id,
            CompanyId = company.Id,
            Title = "Backend <Developer>",
            AdditionalDays = 4,
            EndDate = new DateOnly(2026, 11, 17)
        }, cancellation.Token);

        var arguments = capPublisher.ReceivedCalls().Should().ContainSingle().Subject.GetArguments();
        var subscription = typeof(InterviewStageExtendedSubscriber)
            .GetMethod(nameof(InterviewStageExtendedSubscriber.HandleAsync))!
            .GetCustomAttribute<CapSubscribeAttribute>()!;
        arguments[0].Should().Be(subscription.Name);
        arguments[3].Should().Be(cancellation.Token);
        var json = JsonSerializer.Serialize(arguments[1], JsonSerializerOptions.Web);
        var notification = JsonSerializer.Deserialize<InterviewStageExtendedMessage>(json, JsonSerializerOptions.Web)!;

        var emailSender = Substitute.For<IEmailSender>();
        var subscriber = new InterviewStageExtendedSubscriber(emailSender);
        await subscriber.HandleAsync(notification, cancellation.Token);

        await emailSender.Received(1).Send(Arg.Is<EmailMessage>(email =>
            email.To == user.Email && email.IsBodyHtml &&
            email.Body.Contains("Vettingo &amp; Partners") && email.Body.Contains("Backend &lt;Developer&gt;") &&
            email.Body.Contains("4 gün") && email.Body.Contains("17 Kasım 2026")));
    }
}
