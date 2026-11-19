using DeleteExam;
using FlashMediator;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetExamSituation;

namespace Vettingo.JobService.Application.Protos
{
    public class ExamDeleteService : ExamDeletesService.ExamDeletesServiceBase
    {
        private readonly ILogger<ExamDeleteService> _logger;
        private readonly IMediator _mediator;
        public ExamDeleteService(ILogger<ExamDeleteService> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        public override async Task<DeleteExamResponse> DeleteExam(DeleteExamRequest request, ServerCallContext context)
        {
            _logger.LogInformation("DeleteExam gRPC isteği alındı. ExamId: {ExamId}", request.ExamId);
            var commandRequest = new GetExamSituationQueriesRequest
            {
                ExamId = Guid.Parse(request.ExamId),

            };
            var inUse = await _mediator.Send(commandRequest);
            return new DeleteExamResponse { Success = inUse.InUse };
        }
    
    }
}
