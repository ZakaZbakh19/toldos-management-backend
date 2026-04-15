using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ModularBackend.Application.Abstractions.Events.IntegrationEvents;
using ModularBackend.Application.Abstractions.Events.Notifications;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Application.Abstractions.Persistence.Products;
using ModularBackend.Application.Abstractions.Services.Cache;
using ModularBackend.Application.Abstractions.Services.Files;
using ModularBackend.Application.Abstractions.Services.Identity;
using ModularBackend.Application.Abstractions.Services.Messaging;
using ModularBackend.Application.Abstractions.Services.Persistence;
using ModularBackend.Infrastructure.EventBus;
using ModularBackend.Infrastructure.Helpers;
using ModularBackend.Infrastructure.Messaging;
using ModularBackend.Infrastructure.Messaging.EventBus;
using ModularBackend.Infrastructure.Persistence;
using ModularBackend.Infrastructure.Persistence.Queries;
using ModularBackend.Infrastructure.Persistence.Repositories;
using ModularBackend.Infrastructure.Services.Cache;
using ModularBackend.Infrastructure.Services.Events.Notifications;
using ModularBackend.Infrastructure.Services.FileStorage;
using ModularBackend.Infrastructure.Services.Identity;
using ModularBackend.Infrastructure.Services.Persistence;
using RabbitMQ.Client;
using StackExchange.Redis;
using System.Text;

namespace ModularBackend.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<RedisOptions>()
                .Bind(configuration.GetSection(RedisOptions.SectionName))
                .Validate(x => !string.IsNullOrWhiteSpace(x.ConnectionString), "Redis ConnectionString is required.")
                .ValidateOnStart();

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
                return ConnectionMultiplexer.Connect(options.ConnectionString);
            });

            services.AddScoped<IStoreService, RedisStoreService>();

            services.AddOptions<RabbitMqOptions>()
                .Bind(configuration.GetSection(RabbitMqOptions.SectionName))
                .Validate(x => !string.IsNullOrWhiteSpace(x.UserName), "RabbitMq:UserName is required.")
                .Validate(x => !string.IsNullOrWhiteSpace(x.ExchangeName), "RabbitMq:ExchangeName is required.")
                .Validate(x => !string.IsNullOrWhiteSpace(x.HostName), "RabbitMq:HostName is required.")
                .Validate(x => !string.IsNullOrWhiteSpace(x.Password), "RabbitMq:Password is required.")
                .ValidateOnStart();

            services.AddStackExchangeRedisOutputCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis:PublicEndpoint");
            });

            services.AddSingleton(sp =>
            {
                var _options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
                return new ConnectionFactory
                {
                    HostName = _options.HostName,
                    Port = _options.Port,
                    UserName = _options.UserName,
                    Password = _options.Password
                };
            });

            services.AddSingleton<IConnection>(sp =>
            {
                var factory = sp.GetRequiredService<ConnectionFactory>();
                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            });

            services.AddSingleton<RabbitMqInitializer>();
            services.AddSingleton<IMessagingBus, RabbitMqIntegrationEventBus>();
            services.AddScoped<IProcessedIntegrationEventStore, ProcessedIntegrationEventStore>();
            services.AddScoped<IIntegrationEventDispatcher, IntegrationEventDispatcher>();

            services.AddHostedService<ProductEventsConsumer>();

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

            // UoW + repos (ApplicationDbContext)
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProductRepository, ProductsRepository>();
            services.AddScoped<IProductQuery, ProductsQuery>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddSingleton<IRefreshTokenHasher, RefreshTokenHasher>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<INotificationPublisher, NotificationPublisher>();
            services.AddHostedService<PublisherWorker>();

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
            .AddEntityFrameworkStores<ApplicationDbContext>()
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

            services.AddSingleton<IFileStorageService, AzureFilesStorageService>();
            services.AddScoped<IFileAccessUrlService, AzureBlobAccessUrlService>();
            services.AddScoped<IStorageKeyFactory, StorageKeyFactory>();
            services.AddHostedService<AzureBlobStorageInitializer>();

            services.AddScoped<ICacheInvalidator, OutputCacheInvalidator>();

            return services;
        }
    }
}
