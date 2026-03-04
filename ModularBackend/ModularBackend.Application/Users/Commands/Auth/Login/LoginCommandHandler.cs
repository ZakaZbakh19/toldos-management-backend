using ModularBackend.Application.Abstractions.Identity;
using ModularBackend.Application.Abstractions.Messaging.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Users.Commands.Auth.Login
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
            var token = await _userService.LoginAsync(request.email, request.password, cancellationToken);

            if (string.IsNullOrEmpty(token.Token))
                throw new ArgumentException();
            if (string.IsNullOrEmpty(token.RefreshToken))
                throw new ArgumentException();

            return new AuthResponseCommand(token.Token, token.RefreshToken, token.ExpirateAt);
        }
    }
}
