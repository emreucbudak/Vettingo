using System.Text.Json;
using DotNetCore.CAP;
using FluentAssertions;
using NSubstitute;
using Vettingo.JobService.Application.Messaging;
using Vettingo.JobService.Infrastructure.Messaging;

namespace Vettingo.JobService.UnitTests.Messaging;

public sealed class JobAnalyticsPublisherTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Publisher_Should_Send_Version7_EventId_And_Preserve_An_Existing_EventId(bool hasEventId)
    {
        var cap = Substitute.For<ICapPublisher>();
        var message = new JobAnalyticsDto
        {
            CompanyId = Guid.NewGuid(),
            EventId = hasEventId ? Guid.CreateVersion7() : Guid.Empty
        };
        using var cancellation = new CancellationTokenSource();

        await new JobAnalyticsPublisher(cap).PublishAsync(message, cancellation.Token);

        var arguments = cap.ReceivedCalls().Single().GetArguments();
        arguments[0].Should().Be(JobAnalyticsDto.TopicName);
        arguments[3].Should().Be(cancellation.Token);
        var published = (JobAnalyticsDto)arguments[1]!;
        published.CompanyId.Should().Be(message.CompanyId);
        published.EventId.Version.Should().Be(7);
        if (hasEventId) published.EventId.Should().Be(message.EventId);

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(published, JsonSerializerOptions.Web));
        document.RootElement.EnumerateObject().Select(property => property.Name)
            .Should().BeEquivalentTo("companyId", "eventId");
    }
}
