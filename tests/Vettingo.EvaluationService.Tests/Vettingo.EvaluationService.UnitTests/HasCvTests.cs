using FlashMediator;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Vettingo.EvaluationService.API.Controllers;
using Vettingo.EvaluationService.Application.Features.CQRS.Cv.Query.HasCv;
using Vettingo.EvaluationService.Application.Repository;
using Vettingo.EvaluationService.Application.Validations;

namespace Vettingo.EvaluationService.UnitTests;

public sealed class HasCvTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Query_Should_Return_Repository_Result_And_Forward_User_And_Cancellation(bool exists)
    {
        var userId = Guid.NewGuid();
        using var cancellation = new CancellationTokenSource();
        var repository = Substitute.For<ICvRepository>();
        repository.ExistsByUserIdAsync(userId, cancellation.Token).Returns(exists);

        var result = await new HasCvQueryHandler(repository).Handle(new(userId), cancellation.Token);

        result.Should().Be(exists);
        await repository.Received(1).ExistsByUserIdAsync(userId, cancellation.Token);
    }

    [Fact]
    public void Validator_Should_Reject_Empty_UserId()
    {
        new HasCvQueryRequestValidator().Validate(new HasCvQueryRequest(Guid.Empty)).IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(true, ClaimTypes.NameIdentifier)]
    [InlineData(false, ClaimTypes.NameIdentifier)]
    [InlineData(true, "sub")]
    [InlineData(false, "sub")]
    public async Task Controller_Should_Use_Token_UserId_And_Return_A_Boolean(bool exists, string claimType)
    {
        var userId = Guid.NewGuid();
        using var cancellation = new CancellationTokenSource();
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<HasCvQueryRequest>(), cancellation.Token).Returns(Task.FromResult(exists));

        var controller = Controller(mediator, userId.ToString(), claimType);
        controller.Request.QueryString = new QueryString($"?userId={Guid.NewGuid()}");
        var result = await controller.HasCv(cancellation.Token);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(exists);
        await mediator.Received(1).Send(Arg.Is<HasCvQueryRequest>(request => request.UserId == userId), cancellation.Token);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task Controller_Should_Reject_Missing_Or_Invalid_Token_UserId(string? subject)
    {
        var mediator = Substitute.For<IMediator>();

        var result = await Controller(mediator, subject).HasCv(CancellationToken.None);

        result.Result.Should().BeOfType<UnauthorizedResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    private static EvaluationsController Controller(IMediator mediator, string? subject,
        string claimType = ClaimTypes.NameIdentifier) => new(mediator)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    subject is null ? [] : [new Claim(claimType, subject)], "test"))
            }
        }
    };
}
