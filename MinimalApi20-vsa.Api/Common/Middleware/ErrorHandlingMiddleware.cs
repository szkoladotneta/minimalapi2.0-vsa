using System.Net;
using System.Text.Json;
using FluentValidation;
using MinimalApi20_vsa.Api.Common.Exceptions;

namespace MinimalApi20_vsa.Api.Common.Middleware;

public class ErrorHandlingMiddleware(
    RequestDelegate next,
    ILogger<ErrorHandlingMiddleware> logger,
    IWebHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        var statusCode = HttpStatusCode.InternalServerError;
        var errorMessage = "An unexpected error occurred.";
        object errors = null;
        
        switch (exception)
        {
            case Exceptions.ValidationException appValidationException:
                statusCode = HttpStatusCode.BadRequest;
                errorMessage = "Validation failed";
                errors = appValidationException.Errors.ToList();
                break;
                
            case AppException appException:
                statusCode = (HttpStatusCode)appException.StatusCode;
                errorMessage = appException.Message;
                break;
                
            default:
                logger.LogError(exception, "Unhandled exception");
                errorMessage = environment.IsDevelopment() 
                    ? exception.Message 
                    : "An unexpected error occurred.";
                break;
        }

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            StatusCode = statusCode,
            Message = errorMessage,
            Errors = errors
        };

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}

// Extension method for easier registration
public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}