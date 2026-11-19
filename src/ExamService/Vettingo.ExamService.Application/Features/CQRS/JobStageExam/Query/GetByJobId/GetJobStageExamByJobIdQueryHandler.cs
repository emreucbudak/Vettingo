using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.DTOs;
using Vettingo.ExamService.Application.Exceptions;
using Vettingo.ExamService.Application.UnitOfWork;

namespace Vettingo.ExamService.Application.Features.CQRS.JobStageExam.Query.GetByJobId
{
    public class GetJobStageExamByJobIdQueryHandler(IExamUnitOfWork unitOfWork, ILogger<GetJobStageExamByJobIdQueryHandler> logger) : IRequestHandler<GetJobStageExamByJobIdQueryRequest, GetJobStageExamByJobIdQueryResponse>
    {
        public async Task<GetJobStageExamByJobIdQueryResponse> Handle(GetJobStageExamByJobIdQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetJobStageExamByJobIdQueryHandler));
            var jobStageExam = await unitOfWork.JobStageExamRepository
                .GetJobStageExamByJobIdAndExamIdAsync(request.JobId, request.ExamId, cancellationToken);

            if (jobStageExam is null)
            {
                throw new NotFoundException("İş ilanında istenen sınav bulunamadı");
            }

            if (!jobStageExam.ExamCandidateList.Contains(request.UserId))
            {
                throw new UnauthorizedException("Bu sınava katılamazsınız");
            }

            var exam = jobStageExam.Exam;
            return new GetJobStageExamByJobIdQueryResponse
            {
                Title = exam.Title,
                DurationMinutes = exam.DurationMinutes,
                MultipleChoiceQuestions = exam.GetMultipleChoiceQuestions()
                    .OrderBy(question => question.DisplayOrder)
                    .Select(question => new GetJobStageExamByJobIdMultipleChoiceQuestionDto
                    {
                        Id = question.Id,
                        QuestionText = question.QuestionText,
                        Weight = question.Weight,
                        DisplayOrder = question.DisplayOrder,
                        Options = question.Options.Select(option => new GetJobStageExamByJobIdMultipleChoiceOptionDto
                        {
                            Id = option.Id,
                            OptionText = option.OptionText
                        }).ToList()
                    }).ToList(),
                TrueFalseQuestions = exam.GetTrueFalseQuestions()
                    .OrderBy(question => question.DisplayOrder)
                    .Select(question => new GetJobStageExamByJobIdTrueFalseQuestionDto
                    {
                        Id = question.Id,
                        QuestionText = question.QuestionText,
                        Weight = question.Weight,
                        DisplayOrder = question.DisplayOrder,
                        Options = question.Options.Select(option => new GetJobStageExamByJobIdTrueFalseOptionDto
                        {
                            Id = option.Id,
                            Type = option.Type
                        }).ToList()
                    }).ToList()
            };
        }
    }
}
