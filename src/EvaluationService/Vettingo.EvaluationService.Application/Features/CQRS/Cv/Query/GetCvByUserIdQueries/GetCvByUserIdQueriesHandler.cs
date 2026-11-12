using FlashMediator;
using Vettingo.EvaluationService.Application.DTOs;
using Vettingo.EvaluationService.Application.Exceptions;
using Vettingo.EvaluationService.Application.Repository;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Cv.Query.GetCvByUserIdQueries;

public sealed class GetCvByUserIdQueriesHandler(ICvRepository cvRepository)
    : IRequestHandler<GetCvByUserIdQueriesRequest, GetCvByUserIdQueriesResponse>
{
    public async Task<GetCvByUserIdQueriesResponse> Handle(
        GetCvByUserIdQueriesRequest request,
        CancellationToken cancellationToken)
    {
        var cv = await cvRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("CV bulunamadı.");

        return new GetCvByUserIdQueriesResponse
        {
            TargetRole = cv.TargetRole,
            TargetSalary = cv.TargetSalary,
            Skills = new Dictionary<string, int>(cv.Skills),
            Languages = cv.Languages.Select(language => new CvLanguageDto(language.LanguageName, language.LanguageLevel)).ToArray(),
            Educations = cv.Educations.ToArray(),
            Projects = cv.Projects.ToArray(),
            Experiences = cv.Experiences.ToArray()
        };
    }
}
