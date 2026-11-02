using System.Security.Claims;
using FlashMediator;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Vettingo.AnalyticsService.API.Controllers;
using Vettingo.AnalyticsService.Application.Features.CQRS.CompanyStatics.Query.GetCompanyHiringVolume;
using Vettingo.AnalyticsService.Application.Validations;
using Vettingo.AnalyticsService.Application.Repository;
using Vettingo.AnalyticsService.Domain.Entities;

namespace Vettingo.AnalyticsService.UnitTests.Application;

public sealed class CompanyHiringVolumeTests
{
    [Fact]
    public async Task Endpoint_Uses_Authenticated_Company_And_Forwards_Cancellation()
    {
        var companyId = Guid.NewGuid();
        IEnumerable<GetCompanyHiringVolumeQueryResponse> response = [new() { Year = 2026, Month = 1, HireCount = 30 }];
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetCompanyHiringVolumeQueryRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));
        using var cancellation = new CancellationTokenSource();

        var result = await Controller(mediator, companyId.ToString())
            .GetCompanyHiringVolume(companyId, cancellation.Token);

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeSameAs(response);
        var arguments = mediator.ReceivedCalls().Single().GetArguments();
        ((GetCompanyHiringVolumeQueryRequest)arguments[0]!).CompanyId.Should().Be(companyId);
        arguments[1].Should().Be(cancellation.Token);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Endpoint_Rejects_Empty_Or_Another_Company(bool supplyId)
    {
        var mediator = Substitute.For<IMediator>();
        var result = await Controller(mediator, Guid.NewGuid().ToString())
            .GetCompanyHiringVolume(supplyId ? Guid.NewGuid() : Guid.Empty, CancellationToken.None);
        result.Should().BeOfType<ForbidResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task Endpoint_Rejects_Missing_Or_Invalid_Company_Claim(string? companyId)
    {
        var mediator = Substitute.For<IMediator>();
        var result = await Controller(mediator, companyId)
            .GetCompanyHiringVolume(Guid.Empty, CancellationToken.None);
        result.Should().BeOfType<UnauthorizedResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public async Task HybridCache_Reuses_Result_Across_Requests_And_Isolates_Companies()
    {
        var companyId = Guid.NewGuid();
        var statistic = new CompanyStatistic(companyId);
        var volume = new HiringVolume();
        typeof(HiringVolume).GetProperty(nameof(HiringVolume.Year))!.SetValue(volume, 2026);
        typeof(HiringVolume).GetProperty(nameof(HiringVolume.Month))!.SetValue(volume, 1);
        typeof(HiringVolume).GetProperty(nameof(HiringVolume.HireCount))!.SetValue(volume, 30);
        statistic.HiringVolumes.Add(volume);
        var repository = Substitute.For<IAnalyticsRepository>();
        repository.GetCompanyStatisticByCompanyIdAsync(companyId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<CompanyStatistic?>(statistic));
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(repository);
        services.AddFlashMediator(typeof(GetCompanyHiringVolumeQueryHandler).Assembly);
        services.AddFlashMediatorHybridCache();
        await using var provider = services.BuildServiceProvider();

        await using (var scope = provider.CreateAsyncScope())
        {
            var result = await scope.ServiceProvider.GetRequiredService<IMediator>()
                .Send(new GetCompanyHiringVolumeQueryRequest(companyId));
            result.Single().HireCount.Should().Be(30);
        }
        await using (var scope = provider.CreateAsyncScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var cached = await mediator.Send(new GetCompanyHiringVolumeQueryRequest(companyId));
            cached.Single().HireCount.Should().Be(30);
            var otherCompanyId = Guid.NewGuid();
            var otherCompany = await mediator.Send(new GetCompanyHiringVolumeQueryRequest(otherCompanyId));
            otherCompany.Should().BeEmpty();
            await repository.Received(1).GetCompanyStatisticByCompanyIdAsync(otherCompanyId, Arg.Any<CancellationToken>());
        }
        await repository.Received(1).GetCompanyStatisticByCompanyIdAsync(companyId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public void Validator_Rejects_Empty_CompanyId()
    {
        var validator = new GetCompanyHiringVolumeQueryRequestValidator();
        validator.Validate(new GetCompanyHiringVolumeQueryRequest(Guid.Empty)).IsValid.Should().BeFalse();
        validator.Validate(new GetCompanyHiringVolumeQueryRequest(Guid.NewGuid())).IsValid.Should().BeTrue();
    }

    private static AnalyticsController Controller(IMediator mediator, string? companyId) => new(mediator)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    companyId is null ? [] : [new Claim("companyId", companyId)], "test"))
            }
        }
    };
}
