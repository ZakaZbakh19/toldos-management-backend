using ModularBackend.Application.Abstractions.Identity;
using ModularBackend.Application.Abstractions.Messaging.Mediator;

namespace ModularBackend.Application.Users.Commands.Auth
{
    public class RegisterCommandHandler : IRequestHandler<RegisterRequestCommand, AuthResponseCommand>
    {
        private readonly IAuthService _userService;

        public RegisterCommandHandler(IAuthService userService)
        {
            _userService = userService;
        }

        public async Task<AuthResponseCommand> Handle(RegisterRequestCommand request, CancellationToken cancellationToken)
        {
            var (token, refreshToken, expirateAt) = await _userService.CreateUser(request.name, request.email, request.password);
            return new AuthResponseCommand(token, refreshToken, expirateAt);
        }
    }
}
