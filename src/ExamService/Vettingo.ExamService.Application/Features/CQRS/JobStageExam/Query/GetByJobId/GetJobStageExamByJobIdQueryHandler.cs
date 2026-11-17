using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.DTOs;
using Vettingo.ExamService.Application.Exceptions;
using Vettingo.ExamService.Application.UnitOfWork;

namespace Vettingo.ExamService.Application.Features.CQRS.JobStageExam.Query.GetByJobId
{
    public class GetJobStageExamByJobIdQueryHandler(IExamUnitOfWork unitOfWork, ILogger<GetJobStageExamByJobIdQueryHandler> logger) : IRequestHandler<GetJobStageExamByJobIdQueryRequest, List<GetJobStageExamByJobIdQueryResponse>>
    {
        public async Task<List<GetJobStageExamByJobIdQueryResponse>> Handle(GetJobStageExamByJobIdQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetJobStageExamByJobIdQueryHandler));
            var jobStageExams = await unitOfWork.JobStageExamRepository
                .GetJobStageExamsByJobIdAndExamIdAsync(request.JobId, request.ExamId, cancellationToken);

            if (jobStageExams.Count == 0)
            {
                throw new NotFoundException("İş ilanına ait sınav bulunamadı");
            }

            return jobStageExams
                .Select(jobStageExam => jobStageExam.Exam)
                .Select(exam => new GetJobStageExamByJobIdQueryResponse
                {
                    CompanyId = exam.CompanyId,
                    OwnerType = exam.OwnerType,
                    Title = exam.Title,
                    DurationMinutes = exam.DurationMinutes,
                    MultipleChoiceQuestions = exam.GetMultipleChoiceQuestions()
                        .OrderBy(question => question.DisplayOrder)
                        .Select(question => new GetJobStageExamByJobIdMultipleChoiceQuestionResponse
                        {
                            Id = question.Id,
                            ExamId = question.ExamId,
                            QuestionText = question.QuestionText,
                            Weight = question.Weight,
                            DisplayOrder = question.DisplayOrder,
                            Options = question.Options.Select(option => new GetJobStageExamByJobIdMultipleChoiceOptionResponse
                            {
                                Id = option.Id,
                                OptionText = option.OptionText,
                                IsCorrect = option.IsCorrect
                            }).ToList()
                        }).ToList(),
                    TrueFalseQuestions = exam.GetTrueFalseQuestions()
                        .OrderBy(question => question.DisplayOrder)
                        .Select(question => new GetJobStageExamByJobIdTrueFalseQuestionResponse
                        {
                            Id = question.Id,
                            ExamId = question.ExamId,
                            QuestionText = question.QuestionText,
                            Weight = question.Weight,
                            DisplayOrder = question.DisplayOrder,
                        }).ToList()
                }).ToList();
        }
    }
}
