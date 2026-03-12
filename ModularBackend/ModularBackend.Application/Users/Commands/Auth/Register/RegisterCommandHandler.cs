using ModularBackend.Application.Abstractions.Identity;
using ModularBackend.Application.Abstractions.Messaging.Mediator;

namespace ModularBackend.Application.Users.Commands.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterRequestCommand, AuthResponseDTO>
    {
        private readonly IAuthService _userService;

        public RegisterCommandHandler(IAuthService userService)
        {
            _userService = userService;
        }

        public async Task<AuthResponseDTO> Handle(RegisterRequestCommand request, CancellationToken cancellationToken)
        {
            var token = await _userService.RegisterAsync(request.name, request.email, request.password, cancellationToken);

            if (string.IsNullOrEmpty(token.Token))
                throw new ArgumentException();
            if (string.IsNullOrEmpty(token.RefreshToken))
                throw new ArgumentException();

            return new AuthResponseDTO(token.Token, token.RefreshToken, token.ExpirateAt);
        }
    }
}
