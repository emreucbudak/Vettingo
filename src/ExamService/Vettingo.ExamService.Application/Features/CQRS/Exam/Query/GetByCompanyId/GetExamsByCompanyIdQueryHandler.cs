using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.UnitOfWork;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetByCompanyId;

public sealed class GetExamsByCompanyIdQueryHandler(
    IExamUnitOfWork unitOfWork,
    ILogger<GetExamsByCompanyIdQueryHandler> logger)
    : IRequestHandler<GetExamsByCompanyIdQueryRequest, List<GetExamsByCompanyIdQueryResponse>>
{
    public async Task<List<GetExamsByCompanyIdQueryResponse>> Handle(
        GetExamsByCompanyIdQueryRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetExamsByCompanyIdQueryHandler));
        var exams = await unitOfWork.ExamRepository.GetExamsByCompanyIdAsync(request.CompanyId, cancellationToken);

        var response = exams
            .Where(exam => exam.IsActive)
            .OrderByDescending(exam => exam.CreatedAt)
            .Select(exam => new GetExamsByCompanyIdQueryResponse
            {
                Id = exam.Id,
                Title = exam.Title,
                DurationMinutes = exam.DurationMinutes,
                PassingScore = exam.PassingScore,
                TotalQuestionCount = exam.MultipleChoiceQuestions.Count + exam.TrueFalseQuestions.Count,
                CreatedAt = exam.CreatedAt
            })
            .ToList();

        return response;
    }
}
