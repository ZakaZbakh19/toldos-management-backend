using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ModularBackend.Application.Abstractions.Identity;
using ModularBackend.Application.Abstractions.Persistance;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Application.Identity;
using ModularBackend.Domain.Exceptions;
using ModularBackend.Infrastructure.Models.Identity;
using ModularBackend.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ModularBackend.Infrastructure.Services.Identity
{
    public sealed class AuthService : IAuthService
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly JwtSettings _settings;

        public AuthService(
            UserManager<Users> userManager,
            SignInManager<Users> signInManager,
            ITokenService tokenService,
            IRefreshTokenService refreshTokenService,
            IOptions<JwtSettings> options)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            _settings = options.Value;
        }

        public async Task<TokenAuth> RegisterAsync(string name, string email, string password, CancellationToken ct)
        {
            var user = new Users { UserName = name, Email = email };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                throw new BusinessRuleViolationException(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddClaimsAsync(user, new[]
            {
                new Claim("permission", "products.manager")
            });

            var claims = await _userManager.GetClaimsAsync(user);
            var accessExpiration = DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes);
            var accessToken = _tokenService.GenerateToken(user.Id, user.Email!, accessExpiration, claims);
            var refreshRaw = await _refreshTokenService.IssueAsync(user.Id, ct);

            return new TokenAuth(accessToken, accessExpiration, refreshRaw);
        }

        public async Task<TokenAuth> LoginAsync(string email, string password, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(email)
                        ?? throw new UnauthorizedAccessException("Invalid credentials.");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
            if (!signInResult.Succeeded)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var accessExpiration = DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes);
            var accessToken = _tokenService.GenerateToken(user.Id, user.Email!, accessExpiration, claims);
            var refreshRaw = await _refreshTokenService.IssueAsync(user.Id, ct);

            return new TokenAuth(accessToken, accessExpiration, refreshRaw);
        }

        public async Task<TokenAuth> RefreshAsync(string refreshRaw, CancellationToken ct)
            => await _refreshTokenService.RotateAsync(refreshRaw, ct);

        public async Task LogoutAsync(string refreshRaw, CancellationToken ct)
            => await _refreshTokenService.RevokeAsync(refreshRaw, "Logout", ct);
    }
}
