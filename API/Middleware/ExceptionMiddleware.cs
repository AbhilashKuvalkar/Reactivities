using System;
using System.Text.Json;

// using System.Text.Json;
using Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace API.Middleware;

public class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IHostEnvironment hostEnvironment) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException validationException)
        {
            await HandleValidationExceptionAsync(context, validationException);
        }
        catch (System.Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        logger.LogError(exception, "{ExceptionMessage}",
            exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = hostEnvironment.IsDevelopment()
            ? new AppException(context.Response.StatusCode, exception.Message, exception.StackTrace)
            : new AppException(context.Response.StatusCode, exception.Message, null);

        var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(json);
    }

    private static async Task HandleValidationExceptionAsync(HttpContext context, ValidationException validationException)
    {
        var validationErrors = new Dictionary<string, string[]>();

        if (validationException.Errors is not null)
        {
            foreach (var item in validationException.Errors)
            {
                if (validationErrors.TryGetValue(item.PropertyName, out var values))
                {
                    validationErrors[item.PropertyName] = [.. values, item.ErrorMessage];
                }
                else
                {
                    validationErrors[item.PropertyName] = [item.ErrorMessage];
                }
            }
        }

        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var validationProblemDetails = new ValidationProblemDetails(validationErrors)
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "Validation Error",
            Title = "Validation Error",
            Detail = "See the errors property for more details."
        };

        await context.Response.WriteAsJsonAsync(validationProblemDetails);
    }
}
