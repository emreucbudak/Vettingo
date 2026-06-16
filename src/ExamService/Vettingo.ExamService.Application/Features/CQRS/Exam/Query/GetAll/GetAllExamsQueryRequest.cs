using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetAll
{
    public class GetAllExamsQueryRequest : IRequest<IEnumerable<GetAllExamsQueryResponse>>
    {
    }
}
