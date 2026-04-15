using ModularBackend.Application.Abstractions.Services.Identity;
using ModularBackend.Application.Features.Users;
using ModularBackend.Application.Mediator;

namespace ModularBackend.Application.Features.Users.Register
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
