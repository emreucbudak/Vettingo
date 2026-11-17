using FluentAssertions;
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
        unitOfWork.ExamRepository.Returns(repository);
        repository.GetExamByIdAndCompanyIdAsync(exam.Id, exam.CompanyId!.Value, CancellationToken.None).Returns(exam);
        var handler = new DeleteExamCommandHandler(unitOfWork, NullLogger<DeleteExamCommandHandler>.Instance);
        var request = new DeleteExamCommandRequest { ExamId = exam.Id, CompanyId = Guid.NewGuid() };

        var delete = () => handler.Handle(request, CancellationToken.None);

        await delete.Should().ThrowAsync<NotFoundException>();
        await repository.Received(1).GetExamByIdAndCompanyIdAsync(exam.Id, request.CompanyId, CancellationToken.None);
        repository.DidNotReceive().Delete(Arg.Any<Exam>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_Should_Delete_The_Companys_Own_Exam()
    {
        var companyId = Guid.NewGuid();
        var exam = new Exam();
        exam.CreateExam("Company exam", 60, 70, ExamOwnerType.Company, companyId);
        var repository = Substitute.For<IExamRepository<Exam>>();
        var unitOfWork = Substitute.For<IExamUnitOfWork>();
        unitOfWork.ExamRepository.Returns(repository);
        repository.GetExamByIdAndCompanyIdAsync(exam.Id, companyId, CancellationToken.None).Returns(exam);
        var handler = new DeleteExamCommandHandler(unitOfWork, NullLogger<DeleteExamCommandHandler>.Instance);

        await handler.Handle(new DeleteExamCommandRequest { ExamId = exam.Id, CompanyId = companyId }, CancellationToken.None);

        repository.Received(1).Delete(exam);
        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
