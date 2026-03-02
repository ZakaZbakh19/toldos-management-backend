using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModularBackend.Application.Abstractions.Identity;
using ModularBackend.Application.Abstractions.Persistance;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Domain.Exceptions;
using ModularBackend.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly IdentityUsersDbContext _context;
        private readonly IIdentityUnitOfWork _unitOfWork;

        public AuthService(ITokenService tokenService,
            UserManager<Users> userManager,
            SignInManager<Users> signInManager,
            IdentityUsersDbContext context,
            IIdentityUnitOfWork unitOfWork)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<(string, string, DateTime)> CreateUserAsync(string name, string email, string password)
        {
            var createUserResult = await _userManager.CreateAsync(new Users
            {
                UserName = name,
                Email = email
            });

            if (!createUserResult.Succeeded)
            {
                throw new BusinessRuleViolationException(string.Join(", ", createUserResult.Errors.Select(e => e.Description)));
            }

            var user = await _userManager.FindByEmailAsync(email);
            var (token, resfreshToken, expiredAt) = await GenerateTokenAndRefreshToken(user);

            return (token, resfreshToken, expiredAt);
        }

        public async Task<(string, string, DateTime)> LoginUserAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new BusinessRuleViolationException("Invalid email or password.");
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if (signInResult.Succeeded)
            {
                var (token, resfreshToken, expiredAt) = await GenerateTokenAndRefreshToken(user);
                return (token, resfreshToken, expiredAt);
            }
            else
            {
                throw new BusinessRuleViolationException("Invalid email or password.");
            }
        }

        public async Task<(string, string, DateTime)> RefreshTokenAsync(string refreshToken)
        {
            var result = await _context.RefreshTokens
                .FirstOrDefaultAsync(q => 
                string.Equals(q.RefreshTokenValue, refreshToken, StringComparison.CurrentCultureIgnoreCase));

            if (result is null ||
                result.Active == false ||
                result.Expiration <= DateTime.UtcNow)
            {
                throw new ForbiddenAccessException();
            }

            if (result.Used)
            {
                //_logger.LogWarning("El refresh token del {UserId} ya fue usado. RT={RefreshToken}", refreshToken.UserId, refreshToken.RefreshTokenValue);

                var refreshTokens = await _context.RefreshTokens
                    .Where(q => q.Active && q.Used == false && q.UserId == result.UserId)
                    .ToListAsync();

                foreach (var rt in refreshTokens)
                {
                    rt.Used = true;
                    rt.Active = false;
                }

                await _unitOfWork.SaveChangesAsync();

                throw new ForbiddenAccessException();
            }

            // TODO: Podríamos validar que el Access Token sí corresponde al mismo usuario

            result.Used = true;

            var user = await _context.Users.FindAsync(result.UserId);

            if (user is null)
            {
                throw new ForbiddenAccessException();
            }

            var jwt = await GenerateTokenAndRefreshToken(user);
            return jwt;
        }

        private async Task<(string, string, DateTime)> GenerateTokenAndRefreshToken(Users user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var expirationAt = DateTime.UtcNow.AddHours(1);
            var token = _tokenService.GenerateToken(user.Id, user.Email, expirationAt, claims);
            var newAccessToken = new RefreshToken
            {
                Active = true,
                Expiration = DateTime.UtcNow.AddDays(7),
                RefreshTokenValue = Guid.NewGuid().ToString("N"),
                Used = false,
                UserId = user.Id
            };
            _context.Add(newAccessToken);
            return (token, newAccessToken.RefreshTokenValue, expirationAt);
        }
    }
}
