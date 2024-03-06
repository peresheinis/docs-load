using DocumentService.Shared.Responses.Errors;
using DocumentService.Web.Exceptions;
using DocumentService.Web.Extensions;
using Serilog.Context;
using System.Collections;
using System.Text.Json;

namespace DocumentService.Web.Middlewares;

public class ExceptionMiddleware : ExceptionMiddlewareBase
{
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, RequestDelegate next, IHostEnvironment hostEnvironment)
        : base(logger, next, hostEnvironment)
    {
        _logger = logger;
    }

    public override async Task InvokeAsync(HttpContext httpContext)
    {
        using (LogContext.PushProperty("UserId", httpContext.GetUserId()))
        {
            await base.InvokeAsync(httpContext);
        }
    }
}

public class ExceptionMiddlewareBase
{
    private readonly ILogger _logger;
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _hostEnvironment;

    public ExceptionMiddlewareBase(ILogger logger, RequestDelegate next, IHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _next = next;
        _hostEnvironment = hostEnvironment;
    }

    public virtual async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            switch (exception)
            {
                case AlertingException alertingException:
                    await HandleAlertingException(httpContext, alertingException);
                    break;
                case AppException appException:
                    await HandleAppException(httpContext, appException);
                    break;
                default:
                    await HandleException(httpContext, exception);
                    break;
            }
        }
    }

    public virtual async Task HandleAlertingException(HttpContext context, AlertingException ex)
    {
        var statusCode = ex switch
        {
            ConflictException when ex.Code == ExceptionCodes.EntityNotFound => StatusCodes.Status404NotFound,
            ConflictException when ex.Code == ExceptionCodes.EntityAlreadyExistConflict => StatusCodes.Status409Conflict,
            ConflictException when ex.Code == ExceptionCodes.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };

        using (LogContext.PushProperty("AlertingExceptionDetails", ex.Details))
        {
            _logger.LogWarning(ex, ex.Message);
        }

        await SendErrorResponse(context, ErrorApiResponseHelper.Error(context, ex), statusCode);
    }

    public virtual async Task HandleAppException(HttpContext context, AppException ex)
    {
        var statusCode = ex switch
        {
            _ => StatusCodes.Status500InternalServerError
        };

        _logger.LogError(ex, ex.Message);
        await SendErrorResponse(context, ErrorApiResponseHelper.Error(context, ex), statusCode);
    }

    public virtual async Task HandleException(HttpContext context, Exception ex)
    {
        var (exMessage, userMessage, detail) = ex switch
        {
            ApiResponseException errorApi => ("Не удалось выполнить запрос",
                errorApi.ErrorApiResponse?.Message ?? "Неожиданный ответ от API",
                errorApi.ErrorApiResponse?.Details ?? "Детали ответа не известны"),

            TaskCanceledException _ => ("Timeout", "Запрос был отменен",
                "Превышение допустимого времени выполнения. Попробуйте еще-раз"),
            _ => ComposeDefaultMessage(ex)
        };

        _logger.LogCritical(ex, exMessage);
        await SendErrorResponse(context,
            ErrorApiResponseHelper.Error(context, "Unhandled exception", userMessage, detail),
            StatusCodes.Status500InternalServerError);

        (string, string, string) ComposeDefaultMessage(Exception ex)
        {
            if (_hostEnvironment.IsProduction())
                return ("Unhandled exception", "Что-то пошло не так.", "Мы уже разбираемся с проблемой");

            return ("Unhandled exception", ex.Message, ex.ToString());
        }
    }

    private static Task SendErrorResponse(HttpContext context, ErrorApiResponse apiError, int statusCode)
    {
        var errorMessage = JsonSerializer.Serialize(apiError);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsync(errorMessage);
    }
}

public static class ErrorApiResponseHelper
{
    public static ErrorApiResponse Error(HttpContext context, AlertingException exception)
    {
        return Error(context, exception.Code, exception.Message, exception.Details,
            MapExceptionDictionary(exception));
    }

    public static ErrorApiResponse Error(HttpContext context, AppException exception)
    {
        return Error(context, "AppException", exception.Message, "Мы уже разбираемся с проблемой",
            MapExceptionDictionary(exception));
    }

    public static ErrorApiResponse Error(HttpContext context, string code, string message, string details,
        IDictionary<string, object>? data = null)
    {
        return new ErrorApiResponse(code, context.TraceIdentifier, message, details, data);
    }

    private static IDictionary<string, object>? MapExceptionDictionary(Exception ex)
    {
        IDictionary<string, object>? data = null;

        if (ex.Data.Count > 0)
            data = ex.Data.Cast<DictionaryEntry>().ToDictionary(e => e.Key.ToString() ?? "unknown", e => e.Value);

        return data;
    }
}
