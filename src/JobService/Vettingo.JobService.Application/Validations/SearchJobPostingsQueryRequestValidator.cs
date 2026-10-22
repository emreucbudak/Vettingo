using FluentValidation;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.Search;

namespace Vettingo.JobService.Application.Validations;

public sealed class SearchJobPostingsQueryRequestValidator
    : AbstractValidator<SearchJobPostingsQueryRequest>
{
    public SearchJobPostingsQueryRequestValidator()
    {
        RuleFor(request => request.Title).MaximumLength(200);
        RuleFor(request => request.CityId).GreaterThan(0).When(request => request.CityId.HasValue);
        RuleFor(request => request.EmploymentType).IsInEnum().When(request => request.EmploymentType.HasValue);
        RuleFor(request => request.WorkingModel).IsInEnum().When(request => request.WorkingModel.HasValue);
        RuleFor(request => request.MinimumExperienceYear).GreaterThanOrEqualTo(0)
            .When(request => request.MinimumExperienceYear.HasValue);
        RuleFor(request => request.MaximumExperienceYear).GreaterThanOrEqualTo(0)
            .When(request => request.MaximumExperienceYear.HasValue);
        RuleFor(request => request)
            .Must(request => !request.MinimumExperienceYear.HasValue ||
                !request.MaximumExperienceYear.HasValue ||
                request.MinimumExperienceYear.Value <= request.MaximumExperienceYear.Value)
            .WithMessage("Minimum deneyim yılı maksimum deneyim yılından büyük olamaz.");
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
