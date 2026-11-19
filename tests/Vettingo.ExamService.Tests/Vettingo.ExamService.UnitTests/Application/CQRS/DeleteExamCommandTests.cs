using DeleteExam;
using FluentAssertions;
using Grpc.Core;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Vettingo.ExamService.Application.Exceptions;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Command.DeleteExam;
using Vettingo.ExamService.Application.Repository;
using Vettingo.ExamService.Application.UnitOfWork;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.UnitTests.Application.CQRS;

public sealed class DeleteExamCommandTests
{
    [Fact]
    public async Task Handle_Should_Reject_Another_Companys_Exam()
    {
        var exam = new Exam();
        exam.CreateExam("Company exam", 60, 70, ExamOwnerType.Company, Guid.NewGuid());
        var repository = Substitute.For<IExamRepository<Exam>>();
        var unitOfWork = Substitute.For<IExamUnitOfWork>();
        var client = Substitute.For<ExamDeletesService.ExamDeletesServiceClient>();
        unitOfWork.ExamRepository.Returns(repository);
        repository.GetExamByIdAndCompanyIdAsync(exam.Id, exam.CompanyId!.Value, CancellationToken.None).Returns(exam);
        var handler = new DeleteExamCommandHandler(unitOfWork, NullLogger<DeleteExamCommandHandler>.Instance, client);
        var request = new DeleteExamCommandRequest { ExamId = exam.Id, CompanyId = Guid.NewGuid() };

        var delete = () => handler.Handle(request, CancellationToken.None);

        await delete.Should().ThrowAsync<NotFoundException>();
        await repository.Received(1).GetExamByIdAndCompanyIdAsync(exam.Id, request.CompanyId, CancellationToken.None);
        _ = client.DidNotReceive().DeleteExamAsync(Arg.Any<DeleteExamRequest>());
        repository.DidNotReceive().Update(Arg.Any<Exam>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_Should_Deactivate_The_Companys_Unused_Exam()
    {
        var companyId = Guid.NewGuid();
        var exam = new Exam();
        exam.CreateExam("Company exam", 60, 70, ExamOwnerType.Company, companyId);
        var repository = Substitute.For<IExamRepository<Exam>>();
        var unitOfWork = Substitute.For<IExamUnitOfWork>();
        var client = Substitute.For<ExamDeletesService.ExamDeletesServiceClient>();
        unitOfWork.ExamRepository.Returns(repository);
        repository.GetExamByIdAndCompanyIdAsync(exam.Id, companyId, CancellationToken.None).Returns(exam);
        using var call = new AsyncUnaryCall<DeleteExamResponse>(
            Task.FromResult(new DeleteExamResponse { Success = false }),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });
        client.DeleteExamAsync(Arg.Is<DeleteExamRequest>(request => request.ExamId == exam.Id.ToString())).Returns(call);
        var handler = new DeleteExamCommandHandler(unitOfWork, NullLogger<DeleteExamCommandHandler>.Instance, client);

        await handler.Handle(new DeleteExamCommandRequest { ExamId = exam.Id, CompanyId = companyId }, CancellationToken.None);

        exam.IsActive.Should().BeFalse();
        repository.Received(1).Update(exam);
        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
