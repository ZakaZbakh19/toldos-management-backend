using ModularBackend.Application.Abstractions.Services.Identity;
using ModularBackend.Application.Mediator;

namespace ModularBackend.Application.Features.Users.Logout
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
