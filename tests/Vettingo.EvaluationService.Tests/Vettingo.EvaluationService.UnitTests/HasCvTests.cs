using FlashMediator;
using FluentAssertions;
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
    [InlineData(true)]
    [InlineData(false)]
    public async Task Controller_Should_Forward_Route_UserId_And_Return_A_Boolean(bool exists)
    {
        var userId = Guid.NewGuid();
        using var cancellation = new CancellationTokenSource();
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<HasCvQueryRequest>(), cancellation.Token).Returns(Task.FromResult(exists));

        var result = await new EvaluationsController(mediator).HasCv(userId, cancellation.Token);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(exists);
        await mediator.Received(1).Send(Arg.Is<HasCvQueryRequest>(request => request.UserId == userId), cancellation.Token);
    }

}
