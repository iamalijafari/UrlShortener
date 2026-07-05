using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Common.Exceptions;
using UrlShortener.Domain.Exceptions;

namespace UrlShortener.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await Handle(context, StatusCodes.Status404NotFound,
                "Resource Not Found", ex.Message);
        }
        catch (ValidationException ex)
        {
            await HandleValidation(context, ex);
        }
        catch (DomainException ex)
        {
            await Handle(context, StatusCodes.Status400BadRequest,
                "Business Rule Violation", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            await Handle(context,
                StatusCodes.Status500InternalServerError,
                "Server Error",
                "An unexpected error occurred.");
        }
    }

    private static async Task Handle(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = statusCode
        });
    }

    private static async Task HandleValidation(
        HttpContext context,
        ValidationException ex)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Title = "Validation Failed",
            Detail = "One or more validation errors occurred.",
            Status = 400,
            Extensions =
            {
                ["errors"] = ex.Errors.Select(x => x.ErrorMessage)
            }
        });
    }
}