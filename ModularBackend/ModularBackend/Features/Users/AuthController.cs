using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ModularBackend.Application.Features.Users;
using ModularBackend.Application.Features.Users.Login;
using ModularBackend.Application.Features.Users.Logout;
using ModularBackend.Application.Features.Users.Refresh;
using ModularBackend.Application.Features.Users.Register;
using ModularBackend.Application.Mediator;

namespace ModularBackend.Api.Features.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [EnableRateLimiting("auth-register-ip")]
        [ProducesResponseType<AuthResponseDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken ct)
        {
            var sendRequest = new RegisterRequestCommand(request.name, request.email, request.password);
            var result = await _mediator.Send(sendRequest, ct);
            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [EnableRateLimiting("auth-login-ip")]
        [ProducesResponseType<AuthResponseDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken ct)
        {
            var sendRequest = new LoginRequestCommand(request.Email, request.Password);
            var result = await _mediator.Send(sendRequest, ct);
            return Ok(result);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        [EnableRateLimiting("auth-refresh-ip")]
        [ProducesResponseType<AuthResponseDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto, CancellationToken ct)
        {
            var sendRequest = new RefreshRequestCommand(dto.RefreshRaw);
            var result = await _mediator.Send(sendRequest, ct);
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        [EnableRateLimiting("auth-logout-user")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto dto, CancellationToken ct)
        {
            var sendRequest = new LogoutRequestCommand(dto.refreshRaw);
            await _mediator.Send(sendRequest, ct);
            return NoContent();
        }
    }   
}


