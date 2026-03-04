using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ModularBackend.Application.Abstractions.Identity;
using ModularBackend.Application.Abstractions.Persistance;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Infraestructure.Persistance;
using ModularBackend.Infraestructure.Repositories;
using ModularBackend.Infraestructure.Repositories.Persistance;
using ModularBackend.Infrastructure.Models.Identity;
using ModularBackend.Infrastructure.Persistance;
using ModularBackend.Infrastructure.Services.Identity;
using System.Text;

namespace ModularBackend.Infraestructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
         this IServiceCollection services,
         IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DevConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWorkRepository>();
            services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
            services.AddScoped<IProductQuery, ProductReadRepository>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!)),
                        //ValidIssuer = configuration["AzureAd:Instance"] + configuration["AzureAd:TenantId"],
                        //ValidAudience = configuration["AzureAd:ClientId"]
                    };
                    //options.Audience = configuration["AzureAd:ClientId"];
                    //options.Authority = $"{configuration["AzureAd:Instance"]}{configuration["AzureAd:TenantId"]}";
                });

            services.AddScoped<IUsers, Users>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.Configure<JwtSettings>(
                configuration.GetSection("Jwt"));
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ProductManager", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("permission", "products.manager");
                });

                options.AddPolicy("AdminOnly", policy =>
                {
                    policy.RequireRole("Admin");
                });
            });
            services.AddAuthorizationBuilder();
            services.AddDbContext<IdentityUsersDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("IdentityDevConnection")));
            services.AddIdentityCore<Users>()
                .AddEntityFrameworkStores<IdentityUsersDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
