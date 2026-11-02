using FlashMediator;
using Vettingo.EvaluationService.Application.Repository;
using CvEntity = Vettingo.EvaluationService.Domain.Models.Cv;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Cv.Command.Create;

public sealed class CreateCvCommandHandler(ICvRepository cvRepository) : IRequestHandler<CreateCvCommandRequest>
{
    public async Task Handle(CreateCvCommandRequest request, CancellationToken cancellationToken)
    {
        var cv = new CvEntity();
        cv.Create(request.UserId, request.TargetRole, request.TargetSalary, request.Skills, request.Languages);

        request.Educations?.ForEach(cv.AddEducation);
        request.Projects?.ForEach(cv.AddProject);
        request.Experiences?.ForEach(cv.AddExperience);

        await cvRepository.AddAsync(cv, cancellationToken);
        await cvRepository.SaveChangesAsync(cancellationToken);
    }
}
