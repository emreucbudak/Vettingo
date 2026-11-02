using FlashMediator;
using Vettingo.EvaluationService.Application.Exceptions;
using Vettingo.EvaluationService.Application.Repository;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Cv.Command.Update;

public sealed class UpdateCvCommandHandler(ICvRepository cvRepository) : IRequestHandler<UpdateCvCommandRequest>
{
    public async Task Handle(UpdateCvCommandRequest request, CancellationToken cancellationToken)
    {
        var cv = await cvRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("CV bulunamadı.");

        cvRepository.Update(cv, request.TargetRole, request.TargetSalary,
            request.Skills, request.Languages, request.Educations, request.Projects, request.Experiences);
        await cvRepository.SaveChangesAsync(cancellationToken);
    }
}
