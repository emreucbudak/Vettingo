using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetByCompanyId;
using Vettingo.ExamService.Application.Repository;
using Vettingo.ExamService.Application.UnitOfWork;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.UnitTests.Application.CQRS;

public sealed class GetExamsByCompanyIdQueryTests
{
    [Fact]
    public void CompanyId_Should_Be_Ignored_In_Json()
    {
        var companyId = Guid.NewGuid();
        var request = new GetExamsByCompanyIdQueryRequest(companyId);
        var json = JsonSerializer.Serialize(request);

        json.Should().Be("{}");
        JsonSerializer.Deserialize<GetExamsByCompanyIdQueryRequest>($$"""{"CompanyId":"{{companyId}}"}""")!
            .CompanyId.Should().BeEmpty();
    }

    [Fact]
    public void Cache_Key_Should_Include_Company_Id()
    {
        var companyId = Guid.NewGuid();
        FlashMediator.ICacheableQuery request = new GetExamsByCompanyIdQueryRequest(companyId);

        request.CacheKey.Should().Be($"{companyId}-exams");
        request.Expiration.Should().Be(TimeSpan.FromMinutes(30));
    }

    [Fact]
    public async Task Handler_Should_Query_Company_Exams_And_Map_Question_Counts()
    {
        var companyId = Guid.NewGuid();
        var exam = new Exam();
        exam.CreateExam("Company exam", 60, 70, ExamOwnerType.Company, companyId);
        var question = new MultipleChoiceQuestion();
        question.CreateQuestion(exam.Id, "Question", 1, 1);
        exam.AddMultipleChoiceQuestion(question);
        var trueFalseQuestion = new TrueFalseQuestion();
        trueFalseQuestion.CreateQuestion(exam.Id, "True or false?", 1, 2);
        exam.AddTrueFalseQuestion(trueFalseQuestion);
        IReadOnlyList<Exam> exams = [exam];
        var repository = Substitute.For<IExamRepository<Exam>>();
        var unitOfWork = Substitute.For<IExamUnitOfWork>();
        unitOfWork.ExamRepository.Returns(repository);
        repository.GetExamsByCompanyIdAsync(companyId, CancellationToken.None).Returns(exams);
        var handler = new GetExamsByCompanyIdQueryHandler(unitOfWork, NullLogger<GetExamsByCompanyIdQueryHandler>.Instance);

        var result = await handler.Handle(new GetExamsByCompanyIdQueryRequest(companyId), CancellationToken.None);

        var companyExam = result.Should().ContainSingle().Subject;
        companyExam.Id.Should().Be(exam.Id);
        companyExam.Title.Should().Be(exam.Title);
        companyExam.TotalQuestionCount.Should().Be(2);
        var json = JsonSerializer.SerializeToElement(
            companyExam, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        json.TryGetProperty("companyId", out _).Should().BeFalse();
        json.TryGetProperty("ownerType", out _).Should().BeFalse();
    }
}
