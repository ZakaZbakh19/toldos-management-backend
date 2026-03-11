using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;

namespace ModularBackend.Api.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy("auth-login-ip", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            options.AddPolicy("auth-register-ip", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 3,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            options.AddPolicy("auth-refresh-ip", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            options.AddPolicy("auth-logout-user", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: GetUserPartitionKey(httpContext),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 20,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            options.AddPolicy("authenticated-user-standard", httpContext =>
                RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey: GetUserPartitionKey(httpContext),
                    factory: _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 60,
                        TokensPerPeriod = 60,
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
