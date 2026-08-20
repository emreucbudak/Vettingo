using FlashMediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Login;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.RefreshToken;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Register;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Revoke;
using Vettingo.AuthService.Application.Features.CQRS.Users.Query.GetByEmail;

namespace Vettingo.AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommandRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommandRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommandRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RevokeCommandRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            string? authenticatedEmail = User.FindFirstValue(ClaimTypes.Email)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Email);

            if (!string.Equals(email, authenticatedEmail, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            return Ok(await mediator.Send(new GetUserByEmailQueryRequest { Email = email }));
        }
    }
}