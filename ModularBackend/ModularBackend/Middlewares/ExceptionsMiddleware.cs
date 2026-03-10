using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ModularBackend.Application.Exceptions;
using ModularBackend.Domain.Exceptions;
using System.Linq;

namespace ModularBackend.Api.Middlewares;

using System.Diagnostics;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

public sealed class ExceptionsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionsMiddleware> _logger;

    public ExceptionsMiddleware(
        RequestDelegate next,
        ILogger<ExceptionsMiddleware> logger)
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
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

            _logger.LogError(
                ex,
                "Unhandled exception for {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                traceId);

            await HandleExceptionAsync(context, ex, traceId);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        string traceId)
    {
        context.Response.ContentType = "application/problem+json";

        if (exception is ValidationException ve)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var errors = ve.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            var problem = new ValidationProblemDetails(errors)
            {
                Title = "Validation failed",
                Status = StatusCodes.Status400BadRequest,
                Type = "https://httpstatuses.com/400",
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = traceId;

            await context.Response.WriteAsJsonAsync(problem);
            return;
        }

        var (statusCode, title, detail, type) = exception switch
        {
            NotFoundException => (
                StatusCodes.Status404NotFound,
                "Resource not found",
                exception.Message,
                "https://httpstatuses.com/404"
            ),

            BusinessRuleViolationException => (
                StatusCodes.Status409Conflict,
                "Business rule violation",
                exception.Message,
                "https://httpstatuses.com/409"
            ),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "You are not authorized to perform this action.",
                "https://httpstatuses.com/401"
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "An unexpected error occurred.",
                "https://httpstatuses.com/500"
            )
        };

        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Status = statusCode,
            Detail = detail,
            Type = type,
            Instance = context.Request.Path
        };

        problemDetails.Extensions["traceId"] = traceId;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
