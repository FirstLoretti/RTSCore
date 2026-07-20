using FluentValidation;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using RTSCore.Application.Common;

namespace RTSCore.WebApi.Common;

public class GlobalExeptionHandler(ILogger<GlobalExeptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        logger.LogError(exception, "Произошла ошибка во время обработки запроса");

        var problemDetails = CreateProblemDetails(exception, httpContext);
        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        var hasHandled = exception switch
        {
            ValidationException validationException => HandleValidation(validationException, problemDetails),
            NotFoundException => true,
            _ => false
        };

        if (hasHandled)
        {
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }

        return false;
    }

    private static bool HandleValidation(ValidationException exception, ProblemDetails problemDetails)
    {
        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        problemDetails.Extensions.Add("errors", errors);

        return true;
    }

    private static ProblemDetails CreateProblemDetails(Exception exception, HttpContext httpContext)
    {
        var (statusCode, title) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Ошибка валидации данных"),
            NotFoundException => (StatusCodes.Status404NotFound, "Сущность не найдена"),
            _ => (StatusCodes.Status500InternalServerError, "Ошибка сервера")
        };

        var problemDetails = new ProblemDetails()
        {
            Status = statusCode,
            Title = title,
            Detail = "Один или несколько параметров запроса не прошли проверку",
            Instance = httpContext.Request.Path
        };

        return problemDetails;
    }
}