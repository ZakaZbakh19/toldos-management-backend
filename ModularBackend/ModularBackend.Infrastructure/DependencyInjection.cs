using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ModularBackend.Application.Abstractions.Identity;
using ModularBackend.Application.Abstractions.Persistance;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Application.Abstractions.Persistence.Product;
using ModularBackend.Application.Abstractions.Persistence.Products;
using ModularBackend.Infrastructure.Models.Identity;
using ModularBackend.Infrastructure.Persistance;
using ModularBackend.Infrastructure.Queries;
using ModularBackend.Infrastructure.Repositories.Persistence;
using ModularBackend.Infrastructure.Services.Identity;
using System.Text;

namespace ModularBackend.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Settings fuertemente tipados
            services.AddOptions<JwtSettings>()
                .Bind(configuration.GetSection(JwtSettings.SectionName))
                .Validate(x => !string.IsNullOrWhiteSpace(x.SecretKey), "Jwt:SecretKey is required.")
                .Validate(x => !string.IsNullOrWhiteSpace(x.Issuer), "Jwt:Issuer is required.")
                .Validate(x => !string.IsNullOrWhiteSpace(x.Audience), "Jwt:Audience is required.")
                .Validate(x => x.AccessTokenMinutes > 0, "Jwt:AccessTokenMinutes must be greater than zero.")
                .Validate(x => x.RefreshTokenDays > 0, "Jwt:RefreshTokenDays must be greater than zero.")
                .ValidateOnStart();

            // DbContexts
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DevConnection")));

            services.AddDbContext<IdentityUsersDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("IdentityDevConnection")));

            // UoW + repos (ApplicationDbContext)
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IIdentityUnitOfWork, IdentityUnitOfWorkRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductQuery, ProductQuery>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddSingleton<IRefreshTokenHasher, RefreshTokenHasher>();
            services.AddScoped<ITokenService, TokenService>();

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

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var jwtSettings = services.BuildServiceProvider()
                        .GetRequiredService<IOptions<JwtSettings>>()
                        .Value;

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
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ProductManager", policy =>
                    policy.RequireClaim("permission", "products.manager"));
            });

            return services;
        }
    }
}
