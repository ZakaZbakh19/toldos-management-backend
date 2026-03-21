using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ModularBackend.Application.Abstractions.Events;
using ModularBackend.Application.Abstractions.Identity;
using ModularBackend.Application.Abstractions.Persistance;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Application.Abstractions.Persistence.Product;
using ModularBackend.Application.Abstractions.Persistence.Products;
using ModularBackend.Application.IntegrationEvents;
using ModularBackend.Application.Services;
using ModularBackend.Infrastructure.EventBus;
using ModularBackend.Infrastructure.Events;
using ModularBackend.Infrastructure.Models.Identity;
using ModularBackend.Infrastructure.Outbox;
using ModularBackend.Infrastructure.Persistance;
using ModularBackend.Infrastructure.Persistance.Context;
using ModularBackend.Infrastructure.Queries;
using ModularBackend.Infrastructure.Repositories.Persistence;
using ModularBackend.Infrastructure.Services;
using ModularBackend.Infrastructure.Services.Identity;
using System.Text;
using Azure.Security.KeyVault.Secrets;

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
                .Validate(x => x.SecretKey.Length >= 32, "Jwt:SecretKey must be at least 32 characters.")
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

            services.AddScoped<INotificationPublisher, NotificationPublisher>();
            services.AddSingleton<IIntegrationEventBus, RabbitMqIntegrationEventBus>();
            services.AddHostedService<OutboxPublisherWorker>();

            // Identity
            services.AddIdentityCore<Users>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddEntityFrameworkStores<IdentityUsersDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

            services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .Configure<IOptions<JwtSettings>>((options, jwtOptions) =>
                {
                    var jwt = jwtOptions.Value;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwt.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwt.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwt.SecretKey)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ProductManager", policy =>
                    policy.RequireClaim("permission", "products.manager"));
            });

            services.AddOptions<AzureBlobStorageOptions>()
                .Bind(configuration.GetSection(AzureBlobStorageOptions.SectionName))
                .Validate(x => !string.IsNullOrWhiteSpace(x.ConnectionString), "ConnectionString is required.")
                .Validate(x => !string.IsNullOrWhiteSpace(x.ContainerName), "ContainerName is required.")
                .ValidateOnStart();

            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<AzureBlobStorageOptions>>().Value;
                return new BlobServiceClient(options.ConnectionString);
            });

            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<AzureBlobStorageOptions>>().Value;
                var blobServiceClient = sp.GetRequiredService<BlobServiceClient>();
                return blobServiceClient.GetBlobContainerClient(options.ContainerName);
            });

            services.AddScoped<IFileStorageService, AzureFilesStorageService>();
            services.AddScoped<IFileAccessUrlService, AzureBlobAccessUrlService>();
            services.AddHostedService<AzureBlobStorageInitializer>();

            return services;
        }
    }
}
