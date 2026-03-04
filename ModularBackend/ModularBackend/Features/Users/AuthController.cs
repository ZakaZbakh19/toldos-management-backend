using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModularBackend.Application.Abstractions.Messaging.Mediator;
using ModularBackend.Application.Users.Commands.Auth.Login;
using ModularBackend.Application.Users.Commands.Auth.Refresh;
using ModularBackend.Application.Users.Commands.Auth.Register;

namespace ModularBackend.Api.Features.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var sendRequest = new RegisterRequestCommand(request.name, request.email, request.password);

            var result = await _mediator.Send(sendRequest);
            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var sendRequest = new LoginRequestCommand(request.Email, request.Password);

            var result = await _mediator.Send(sendRequest);
            return Ok(result);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] string refreshRaw)
        {
            var sendRequest = new RefreshRequestCommand(refreshRaw);
            var result = await _mediator.Send(sendRequest);
            return Ok(result);
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout([FromBody] string refreshRaw)
        {
            var sendRequest = new RefreshRequestCommand(refreshRaw);
            var result = await _mediator.Send(sendRequest);
            return Ok(result);
        }
    }   
}


