using System.ComponentModel.DataAnnotations;
using TimeRecord.Exceptions;
using TimeRecord.Models;

namespace TimeRecord.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            catch (Exception error)
            {
                _logger.LogError(error, "Error unexpected: {message}", error.Message);
                await HandleExceptionAsync(context, error);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";

            var (status, title) = exception switch
            {
                KeyNotFoundException => (401, exception.Message), NotFoundException => (404, exception.Message),
                UnauthorizedAccessException => (401, exception.Message),
                AppException=>(404, exception.Message),
                ValidationException => (400, exception.Message),
                _ => (500, exception.Message)
            };

            context.Response.StatusCode = status;

            var problem = new ProblemDetails
            {
                StatusCode = status,
                Title = title,
            };

            return context.Response.WriteAsJsonAsync(problem);
        }
    }
}