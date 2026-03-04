using ModularBackend.Application.Abstractions.Identity;
using ModularBackend.Application.Abstractions.Messaging.Mediator;
using ModularBackend.Application.Users.Commands.Auth.Refresh;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Users.Commands.Auth.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutRequestCommand, Unit>
    {
        private readonly IAuthService _authService;

        public LogoutCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Unit> Handle(LogoutRequestCommand request, CancellationToken cancellationToken)
        {
            await _authService.LogoutAsync(request.refreshRaw, cancellationToken);

            return Unit.Instance;
        }
    }
}
