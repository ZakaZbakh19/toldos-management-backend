using ModularBackend.Application.Abstractions.Identity;
using ModularBackend.Application.Abstractions.Messaging.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Users.Commands.Auth
{
    public class LoginCommandHandler : IRequestHandler<LoginRequestCommand, AuthResponseCommand>
    {
        private readonly IAuthService _userService;

        public LoginCommandHandler(IAuthService userService)
        {
            _userService = userService;
        }

        public async Task<AuthResponseCommand> Handle(LoginRequestCommand request, CancellationToken cancellationToken)
        {
            var (token, refreshToken, expirateAt) = await _userService.LoginUser(request.email, request.password);
            return new AuthResponseCommand(token, refreshToken, expirateAt);
        }
    }
}
