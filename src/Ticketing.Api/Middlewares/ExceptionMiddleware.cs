namespace Ticketing.Api.Middlewares
{
    using Microsoft.AspNetCore.Mvc;
    using Ticketing.Application.Exceptions;
    using Ticketing.Domain.Exceptions;

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                var traceId = context.TraceIdentifier;

                _logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}", traceId);

                context.Response.ContentType = "application/json";

                var statusCode = ex switch
                {
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                    ForbiddenException => StatusCodes.Status403Forbidden,
                    DomainException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError,
                };

                context.Response.StatusCode = statusCode;

                var problem = new ProblemDetails
                {
                    Title = "Une erreur est survenue",
                    Status = statusCode,
                    Detail = ex.Message,
                    Instance = context.Request.Path,
                };

                problem.Extensions["traceId"] = traceId;

                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }
}
