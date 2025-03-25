using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SharedLibrary.Middlewares.GlobalExceptionHandler;
using System.Text.Json;

namespace SharedLibrary.Filters;

public class CustomValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ModelState.IsValid)
        {
            await next();
            return;
        }

        if (HasJsonParsingError(context.ModelState))
        {
            var error = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .FirstOrDefault(e => e.Exception is JsonException);

            if (error?.Exception is JsonException jsonException)
            {
                // Re-throw the original JsonException
                throw jsonException;
            }

            var errorMessage = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault(msg => msg.Contains("JSON") || msg.Contains("unexpected"));

            throw new JsonException(errorMessage ?? "Invalid JSON format in request body");
        }
        // Get first error
        var errors = context.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .Select(e => new
            {
                PropertyName = e.Key,
                ErrorMessage = e.Value.Errors.First().ErrorMessage
            }).ToList();

        if (errors.Any())
        {
            throw new BusinessException(errors.First().ErrorMessage);
        }

        throw new BusinessException("Validation failed. Please check your input and try again.");
    }

    private bool HasJsonParsingError(ModelStateDictionary modelState)
    {
        return modelState.Keys.Any(k => k == "$") || // Root JSON errors
               modelState.Values.Any(v => v.Errors.Any(e =>
                   e.ErrorMessage.Contains("JSON") ||
                   e.ErrorMessage.Contains("unexpected") ||
                   e.Exception is JsonException));
    }
}