using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetByCompanyId;
using Vettingo.ExamService.Application.Repository;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.UnitTests.Application.CQRS;

public sealed class GetExamsByCompanyIdQueryTests
{
    [Fact]
    public void CompanyId_Should_Be_Ignored_In_Json()
    {
        var companyId = Guid.NewGuid();
        var request = new GetExamsByCompanyIdQueryRequest { CompanyId = companyId };
        var json = JsonSerializer.Serialize(request);

        json.Should().Be("{}");
        JsonSerializer.Deserialize<GetExamsByCompanyIdQueryRequest>($$"""{"CompanyId":"{{companyId}}"}""")!
            .CompanyId.Should().BeEmpty();
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
        trueFalseQuestion.CreateQuestion(exam.Id, "True or false?", 1, 2, "True", "False");
        exam.AddTrueFalseQuestion(trueFalseQuestion);
        IReadOnlyList<Exam> exams = [exam];
        var repository = Substitute.For<IExamRepository<Exam>>();
        repository.GetExamsByCompanyIdAsync(companyId, CancellationToken.None).Returns(exams);
        var handler = new GetExamsByCompanyIdQueryHandler(repository, NullLogger<GetExamsByCompanyIdQueryHandler>.Instance);

        var result = await handler.Handle(new GetExamsByCompanyIdQueryRequest { CompanyId = companyId }, CancellationToken.None);

        var companyExam = result.Should().ContainSingle().Subject;
        companyExam.Id.Should().Be(exam.Id);
        companyExam.CompanyId.Should().Be(companyId);
        companyExam.Title.Should().Be(exam.Title);
        companyExam.TotalQuestionCount.Should().Be(2);
    }
}
