using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ModularBackend.Application.Abstractions.Identity;
using ModularBackend.Application.Abstractions.Persistance;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Infrastructure.Persistance;
using ModularBackend.Infrastructure.Repositories.Persistance;
using ModularBackend.Infrastructure.Models.Identity;
using ModularBackend.Infrastructure.Services.Identity;
using System.Text;
using ModularBackend.Infrastructure.Repositories.Persistence;

namespace ModularBackend.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Settings fuertemente tipados
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()
                             ?? throw new InvalidOperationException("Missing Jwt settings.");

            // DbContexts
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DevConnection")));

            services.AddDbContext<IdentityUsersDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("IdentityDevConnection")));

            // UoW + repos (ApplicationDbContext)
            services.AddScoped<IUnitOfWork, UnitOfWorkRepository>();
            services.AddScoped<IIdentityUnitOfWork, IdentityUnitOfWorkRepository>();
            services.AddScoped<IProductWriteRepository, ProductRepository>();
            services.AddScoped<IProductReadRepository, ProductReadRepository>();

            // Identity
            services.AddIdentityCore<Users>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddEntityFrameworkStores<IdentityUsersDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

            // JWT AuthN
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                });

            // Policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ProductManager", p =>
                {
                    p.RequireAuthenticatedUser();
                    p.RequireClaim("permission", "products.manager");
                });

                options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
            });

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRefreshTokenHasher, RefreshTokenHasher>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
