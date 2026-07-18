using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net;
public static class GlobalExceptionHandler
{
    public static void Configure(IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("GlobalExceptionHandler");
                var logService = context.RequestServices.GetRequiredService<ILogService>();

                var (statusCode, message) = exception switch
                {
                    NotFoundException => (HttpStatusCode.NotFound, exception.Message),
                    ValidationException => (HttpStatusCode.BadRequest, exception.Message),
                    DuplicateException => (HttpStatusCode.Conflict, exception.Message),
                    ArgumentNullException => (HttpStatusCode.BadRequest, "Invalid request data"),
                    ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
                    UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized access"),
                    _ => (HttpStatusCode.InternalServerError, "An internal error occurred")
                };

                if (statusCode == HttpStatusCode.InternalServerError && exception != null)
                {
                    logService.Log(LogLevel.Error, exception.Message);
                    logger.LogError(exception, "An Unhandled error occurred: {Message}", exception.Message);
                }
                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    status = (int)statusCode,
                    error = message,
                    timestamp = DateTime.UtcNow
                };

                await context.Response.WriteAsJsonAsync(response);
            });
        });
    }
}