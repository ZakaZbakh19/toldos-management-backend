using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace ModularBackend.Api.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            var loginLimit = configuration.GetValue<int>("RateLimiting:AuthLoginPermitLimit");
            var registerLimit = configuration.GetValue<int>("RateLimiting:AuthRegisterPermitLimit");
            var refreshLimit = configuration.GetValue<int>("RateLimiting:AuthRefreshPermitLimit");
            var logoutLimit = configuration.GetValue<int>("RateLimiting:AuthLogoutPermitLimit");
            var authLimit = configuration.GetValue<int>("RateLimiting:AuthPermitLimit");

            options.AddPolicy("auth-login-ip", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = loginLimit,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            options.AddPolicy("auth-register-ip", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = registerLimit,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            options.AddPolicy("auth-refresh-ip", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = refreshLimit,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            options.AddPolicy("auth-logout-user", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: GetUserPartitionKey(httpContext),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = logoutLimit,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            options.AddPolicy("authenticated-user-standard", httpContext =>
                RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey: GetUserPartitionKey(httpContext),
                    factory: _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = authLimit,
                        TokensPerPeriod = authLimit,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            options.OnRejected = async (context, token) =>
            {
                var httpContext = context.HttpContext;

                httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                httpContext.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Title = "Too many requests",
                    Status = StatusCodes.Status429TooManyRequests,
                    Detail = "You have exceeded the allowed request rate.",
                    Type = "https://httpstatuses.com/429",
                    Instance = httpContext.Request.Path
                };

                problem.Extensions["traceId"] = httpContext.TraceIdentifier;

                await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken: token);
            };
        });

        return services;
    }

    private static string GetUserPartitionKey(HttpContext httpContext)
    {
        return httpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
               ?? httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? httpContext.User.Identity?.Name
               ?? "anonymous";
    }
}
