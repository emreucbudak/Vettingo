using FlashMediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vettingo.AuthService.Application.Features.CQRS.Company.Command.CreateCompany;
using Vettingo.AuthService.Application.Features.CQRS.Company.Command.DeleteCompany;
using Vettingo.AuthService.Application.Features.CQRS.Company.Command.UpdateCompany;
using Vettingo.AuthService.Application.Features.CQRS.Company.Query.GetAll;
using Vettingo.AuthService.Application.Features.CQRS.Company.Query.GetById;

namespace Vettingo.AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController(IMediator mediator) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
            return Ok(await mediator.Send(new GetAllCompaniesQueryRequest()));
        }

        [Authorize(Roles = "Admin,Company")]
        [HttpGet("{companyId:guid}")]
        public async Task<IActionResult> GetCompanyById([FromRoute] Guid companyId)
        {
            return Ok(await mediator.Send(new GetCompanyByIdQueriesRequest { CompanyId = companyId }));
        }
        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyCommandRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }

        [Authorize(Roles = "Admin,Company")]
        [HttpPut("{companyId:guid}")]
        public async Task<IActionResult> UpdateCompany([FromRoute] Guid companyId, [FromBody] UpdateCompanyCommandRequest request)
        {
            await mediator.Send(new UpdateCompanyCommandRequest
            {
                CompanyId = companyId,
                CompanyName = request.CompanyName,
                CompanyDescription = request.CompanyDescription,
                CompanyPhone = request.CompanyPhone,
                CompanyEmail = request.CompanyEmail,
                CompanyAddress = request.CompanyAddress
            });

            return Ok();
        }

        [Authorize(Roles = "Admin,Company")]
        [HttpDelete("{companyId:guid}")]
        public async Task<IActionResult> DeleteCompany([FromRoute] Guid companyId)
        {
            await mediator.Send(new DeleteCompanyCommandRequest { CompanyId = companyId });
            return Ok();
        }
    }
}
