using FlashMediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vettingo.JobService.Application.Features.CQRS.City.Query.GetAll;

namespace Vettingo.JobService.API.Controllers;

[ApiController]
[Route("api/cities")]
[Authorize]
public sealed class CitiesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCities(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetAllCitiesQueryRequest(), cancellationToken));
}
