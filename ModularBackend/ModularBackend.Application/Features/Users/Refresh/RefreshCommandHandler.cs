using ModularBackend.Application.Abstractions.Services.Identity;
using ModularBackend.Application.Features.Users;
using ModularBackend.Application.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Features.Users.Refresh
{
    public class RefreshCommandHandler : IRequestHandler<RefreshRequestCommand, AuthResponseDTO>
    {
        private readonly IAuthService _authService;

        public RefreshCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResponseDTO> Handle(RefreshRequestCommand request, CancellationToken cancellationToken)
        {
            var token = await _authService.RefreshAsync(request.refreshRaw, cancellationToken);

            if (string.IsNullOrEmpty(token.Token))
                throw new ArgumentException();
            if (string.IsNullOrEmpty(token.RefreshToken))
                throw new ArgumentException();

            return new AuthResponseDTO(token.Token, token.RefreshToken, token.ExpirateAt);
        }
    }
}
