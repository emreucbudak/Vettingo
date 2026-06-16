using FlashMediator;
using Microsoft.AspNetCore.Mvc;

namespace Vettingo.AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
            return Ok(await mediator.Send(new Vettingo.AuthService.Application.Features.CQRS.Company.Query.GetAllCompanies.GetAllCompaniesQueryRequest()));
        }
        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] Vettingo.AuthService.Application.Features.CQRS.Company.Command.CreateCompany.CreateCompanyCommandRequest request)
        {
            await mediator.Send(request);
            return Ok();

        }
        [HttpPut]
        public async Task<IActionResult> UpdateCompany([FromBody] Vettingo.AuthService.Application.Features.CQRS.Company.Command.UpdateCompany.UpdateCompanyCommandRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCompany([FromBody] Vettingo.AuthService.Application.Features.CQRS.Company.Command.DeleteCompany.DeleteCompanyCommandRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetCompanyById([FromQuery] Vettingo.AuthService.Application.Features.CQRS.Company.Query.GetCompanyById.GetCompanyByIdQueryRequest request)
        {
            return Ok(await mediator.Send(request));
        }
    }
}
