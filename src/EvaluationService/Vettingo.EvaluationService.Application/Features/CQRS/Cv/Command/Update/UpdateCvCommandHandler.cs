using FlashMediator;
using Vettingo.EvaluationService.Application.Exceptions;
using Vettingo.EvaluationService.Application.Repository;
using Vettingo.EvaluationService.Domain.Models;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Cv.Command.Update;

public sealed class UpdateCvCommandHandler(ICvRepository cvRepository) : IRequestHandler<UpdateCvCommandRequest>
{
    public async Task Handle(UpdateCvCommandRequest request, CancellationToken cancellationToken)
    {
        var cv = await cvRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("CV bulunamadı.");

        var languages = request.Languages?.Select(language => new Language(language.LanguageName, language.LanguageLevel)).ToArray();
        cvRepository.Update(cv, request.TargetRole, request.TargetSalary,
            languages, request.Educations, request.Projects, request.Experiences);
        await cvRepository.SaveChangesAsync(cancellationToken);
    }
}
