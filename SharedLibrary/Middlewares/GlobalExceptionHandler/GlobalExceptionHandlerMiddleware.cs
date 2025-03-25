using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Net;
using System.Text.Json;

namespace SharedLibrary.Middlewares.GlobalExceptionHandler;

public sealed class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        // If there is an error that I don't recognize
        var title = "Internal Server Error.";
        var statusCode = HttpStatusCode.InternalServerError;
        var detail = "An unhandled error occurred.";
        var type = exception.GetType().Name;

        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var globalException = new GlobalException
        {
            Title = title,
            Status = statusCode,
            Type = exception.GetType().Name,
            Detail = detail
        };

        switch (exception)
        {
            case BusinessException businessException:
                title = "Business Related Exception";
                detail = businessException.ExceptionMessage;
                statusCode = HttpStatusCode.BadRequest;
                type = businessException.GetType().Name;
                break;

            // Invalid incoming HTTP request.
            case ArgumentException argumentException:
                title = "Data Related Argument Exception";
                detail = exception.Message;
                statusCode = HttpStatusCode.BadRequest;
                type = argumentException.GetType().Name;
                break;
            case InvalidDataException invalidDataException:
                title = "Invalid Data Exception";
                detail = exception.Message;
                statusCode = HttpStatusCode.BadRequest;
                type = invalidDataException.GetType().Name;
                break;
            case JsonException jsonException_:
                title = "JSON Parsing Exception";
                detail = exception.Message;
                statusCode = HttpStatusCode.BadRequest;
                type = jsonException_.GetType().Name;
                break;

            case InvalidOperationException invalidOperationException when invalidOperationException.Message.Contains("LINQ"):
                title = "Invalid Operation Exception ";
                detail = "The specified query operator or function is not supported.";
                statusCode = HttpStatusCode.BadRequest;
                type = invalidOperationException.GetType().Name;
                break;

            // Concurrency violation during DB update or delete.
            case DBConcurrencyException dBConcurrencyException:
                title = "Database Concurrency Exception ";
                detail = exception.Message;
                statusCode = HttpStatusCode.Conflict;
                type = dBConcurrencyException.GetType().Name;
                break;

            // if an HttpRequestException bubbles up till here we assume we were trying to reach some external service and failed.
            case HttpRequestException httpRequestException:
                title = "Http Request Exception";
                detail = exception.Message;
                statusCode = HttpStatusCode.BadGateway;
                type = httpRequestException.GetType().Name;
                break;
        }

        globalException.Title = title;
        globalException.Status = statusCode;
        globalException.Detail = detail;
        globalException.Type = type;

        httpContext.Response.StatusCode = (int)globalException.Status;
        httpContext.Response.ContentType = "application/json";

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(globalException, jsonOptions);

        await httpContext.Response.WriteAsync(json);
    }
}