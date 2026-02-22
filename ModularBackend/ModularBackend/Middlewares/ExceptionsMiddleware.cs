using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ModularBackend.Application.Exceptions;
using ModularBackend.Domain.Exceptions;
using System.Linq;

namespace ModularBackend.Api.Middlewares;

public sealed class ExceptionsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionsMiddleware> _logger;

    public ExceptionsMiddleware(RequestDelegate next, ILogger<ExceptionsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}", context.TraceIdentifier);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var traceId = context.TraceIdentifier;

        if (exception is ValidationException ve)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var errors = ve.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            var problem = new ValidationProblemDetails(errors)
            {
                Title = "Validation failed",
                Status = StatusCodes.Status400BadRequest,
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = traceId;

            await context.Response.WriteAsJsonAsync(problem);
            return;
        }

        var (statusCode, title, type) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Resource not found", "not_found"),
            BusinessRuleViolationException => (StatusCodes.Status409Conflict, exception.Message, "business_rule_violation"),
            _ => (StatusCodes.Status500InternalServerError, "Unexpected error", "server_error")
        };

        context.Response.StatusCode = statusCode;

        var pd = new ProblemDetails
        {
            Title = title,
            Status = statusCode,
            Type = type,
            Detail = statusCode == 500 ? "An unexpected error occurred." : exception.Message,
            Instance = context.Request.Path
        };

        pd.Extensions["traceId"] = traceId;

        await context.Response.WriteAsJsonAsync(pd);
    }
}
