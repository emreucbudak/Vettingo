using System.Security.Claims;
using FlashMediator;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Vettingo.AnalyticsService.API.Controllers;
using Vettingo.AnalyticsService.Application.Features.CQRS.CompanyStatics.Query.GetCompanyHiringVolume;
using Vettingo.AnalyticsService.Application.Validations;

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
