using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Exceptions;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.JobExam.Query.GetByJobId
{
    public class GetJobExamByJobIdQueryHandler(IExamRepository<Domain.Entities.JobExam> jobExamRepository, IExamRepository<Domain.Entities.Exam> examRepository, ILogger<GetJobExamByJobIdQueryHandler> logger) : IRequestHandler<GetJobExamByJobIdQueryRequest, GetJobExamByJobIdQueryResponse>
    {
        public async Task<GetJobExamByJobIdQueryResponse> Handle(GetJobExamByJobIdQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetJobExamByJobIdQueryHandler));
            var jobExams = await jobExamRepository.GetAllAsync();
            var jobExam = jobExams.FirstOrDefault(item => item.JobId == request.JobId)
                ?? throw new NotFoundException("İş ilanına ait sınav bulunamadı");
            var exam = await examRepository.GetByIdAsync(jobExam.ExamId)
                ?? throw new NotFoundException("Sınav bulunamadı");

            return new GetJobExamByJobIdQueryResponse
            {
                CompanyId = exam.CompanyId,
                OwnerType = exam.OwnerType,
                Title = exam.Title,
                Description = exam.Description,
                DurationMinutes = exam.DurationMinutes,
                MultipleChoiceQuestions = exam.GetMultipleChoiceQuestions()
                    .OrderBy(question => question.DisplayOrder)
                    .Select(question => new GetJobExamByJobIdMultipleChoiceQuestionResponse
                    {
                        Id = question.Id,
                        ExamId = question.ExamId,
                        QuestionText = question.QuestionText,
                        Weight = question.Weight,
                        DisplayOrder = question.DisplayOrder,
                        Options = question.Options.Select(option => new GetJobExamByJobIdMultipleChoiceOptionResponse
                        {
                            Id = option.Id,
                            OptionText = option.OptionText,
                            IsCorrect = option.IsCorrect
                        }).ToList()
                    }).ToList(),
                TrueFalseQuestions = exam.GetTrueFalseQuestions()
                    .OrderBy(question => question.DisplayOrder)
                    .Select(question => new GetJobExamByJobIdTrueFalseQuestionResponse
                    {
                        Id = question.Id,
                        ExamId = question.ExamId,
                        QuestionText = question.QuestionText,
                        Weight = question.Weight,
                        DisplayOrder = question.DisplayOrder,
                        TrueAnswer = question.TrueAnswer,
                        FalseAnswer = question.FalseAnswer
                    }).ToList()
            };
        }
    }
}
