using FluentValidation;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.Search;

namespace Vettingo.JobService.Application.Validations;

public sealed class SearchJobPostingsQueryRequestValidator
    : AbstractValidator<SearchJobPostingsQueryRequest>
{
    public SearchJobPostingsQueryRequestValidator()
    {
        RuleFor(request => request.Title).MaximumLength(200);
        RuleFor(request => request.Location).MaximumLength(200);
        RuleFor(request => request.EmploymentType).IsInEnum().When(request => request.EmploymentType.HasValue);
        RuleFor(request => request.WorkingModel).IsInEnum().When(request => request.WorkingModel.HasValue);
        RuleFor(request => request.ExperienceLevel).IsInEnum().When(request => request.ExperienceLevel.HasValue);
        RuleFor(request => request.MinSalary).GreaterThanOrEqualTo(0).When(request => request.MinSalary.HasValue);
        RuleFor(request => request.MaxSalary).GreaterThanOrEqualTo(0).When(request => request.MaxSalary.HasValue);
        RuleFor(request => request)
            .Must(request =>
                !request.MinSalary.HasValue ||
                !request.MaxSalary.HasValue ||
                request.MinSalary.Value <= request.MaxSalary.Value)
            .WithMessage("Minimum maaş maksimum maaştan büyük olamaz.");
    }
}
