using FlashMediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ChangePassword;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Login;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.RefreshToken;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Register;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Revoke;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TempRegister;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TwoFactorLogin;
using Vettingo.AuthService.Application.Features.CQRS.Users.Query.GetByEmail;

namespace Vettingo.AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommandRequest request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpPost("temp-register")]
        public async Task<IActionResult> TempRegister(
            [FromBody] TempRegisterCommandRequest request,
            CancellationToken cancellationToken)
        {
            return Ok(await mediator.Send(request, cancellationToken));
        }

        [Authorize(Roles = "TempRegister")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterCommandRequest request,
            CancellationToken cancellationToken)
        {
            string? email = User.FindFirstValue(ClaimTypes.Email)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Email);
            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

            await mediator.Send(request with { Email = email }, cancellationToken);
            return NoContent();
        }

        [Authorize(Roles = "TempLogin")]
        [HttpPost("two-factor-login")]
        public async Task<IActionResult> TwoFactorLogin(
            [FromBody] TwoFactorLoginCommandRequest request,
            CancellationToken cancellationToken)
        {
            string? email = User.FindFirstValue(ClaimTypes.Email)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Email);
            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

            return Ok(await mediator.Send(request with { Email = email }, cancellationToken));
        }

        [Authorize(Roles = "Candidate,Company,Human Resources")]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordCommandRequest request,
            CancellationToken cancellationToken)
        {
            string? email = User.FindFirstValue(ClaimTypes.Email)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Email);
            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

            await mediator.Send(request with { Email = email }, cancellationToken);
            return NoContent();
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
