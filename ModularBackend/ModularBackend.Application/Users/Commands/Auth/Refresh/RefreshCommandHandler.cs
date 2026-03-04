using ModularBackend.Application.Abstractions.Identity;
using ModularBackend.Application.Abstractions.Messaging.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Users.Commands.Auth.Refresh
{
    public class RefreshCommandHandler : IRequestHandler<RefreshRequestCommand, AuthResponseCommand>
    {
        private readonly IAuthService _authService;

        public RefreshCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResponseCommand> Handle(RefreshRequestCommand request, CancellationToken cancellationToken)
        {
            var token = await _authService.RefreshAsync(request.refreshRaw, cancellationToken);

            if (string.IsNullOrEmpty(token.Token))
                throw new ArgumentException();
            if (string.IsNullOrEmpty(token.RefreshToken))
                throw new ArgumentException();

            return new AuthResponseCommand(token.Token, token.RefreshToken, token.ExpirateAt);
        }
    }
}
