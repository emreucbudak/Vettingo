using System.Reflection;
using System.Security.Claims;
using FlashMediator;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Vettingo.JobService.API.Controllers;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetByCompany;

namespace Vettingo.JobService.UnitTests.Application;

public sealed class CompanyJobPostingsControllerTests
{
    [Fact]
    public async Task List_Should_Use_Token_Company_And_Forward_Limit()
    {
        var companyId = Guid.NewGuid();
        var mediator = Substitute.For<IMediator>();
        var controller = Controller(mediator, companyId.ToString());

        var result = await controller.GetCompanyJobPostings(CancellationToken.None, 3);

        result.Should().BeOfType<OkObjectResult>();
        var request = (GetCompanyJobPostingsQueryRequest)mediator.ReceivedCalls().Single().GetArguments()[0]!;
        request.CompanyId.Should().Be(companyId);
        request.Limit.Should().Be(3);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task List_Should_Reject_Missing_Or_Invalid_Company(string? companyId)
    {
        var mediator = Substitute.For<IMediator>();
        var result = await Controller(mediator, companyId).GetCompanyJobPostings(CancellationToken.None);
        result.Should().BeOfType<UnauthorizedResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public void List_Should_Allow_Company_And_Hr_Roles()
    {
        var attribute = typeof(JobPostingController).GetMethod(nameof(JobPostingController.GetCompanyJobPostings))!
            .GetCustomAttribute<AuthorizeAttribute>()!;
        attribute.Roles!.Split(',').Should().BeEquivalentTo("Company", "Human Resources");
    }

    private static JobPostingController Controller(IMediator mediator, string? companyId) => new(mediator)
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
