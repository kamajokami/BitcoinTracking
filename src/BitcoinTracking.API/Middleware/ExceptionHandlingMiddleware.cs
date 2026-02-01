using System.Net;
using System.Text.Json;
using BitcoinTracking.BAL.Exceptions;

namespace BitcoinTracking.API.Middleware
{
    /// <summary>
    /// Middleware for centralized exception handling
    /// Catches all exceptions and returns consistent error responses
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        /// <summary>
        /// Delegate representing the next middleware in the request pipeline.
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// Logger instance used for recording exception details.
        /// </summary>
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of ExceptionHandlingMiddleware class.
        /// </summary>
        /// <param name="next">Next middleware in HTTP pipeline.</param>
        /// <param name="logger">Logger used for exception diagnostics.</param>
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }


        /// <summary>
        /// Executes middleware logic.
        /// Wraps request pipeline execution in a try/catch block to intercept unhandled exceptions.
        /// </summary>
        /// <param name="context">Current HTTP request context.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continue processing HTTP request
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");

                // Convert exception into HTTP response
                await HandleExceptionAsync(context, ex);
            }
        }


        /// <summary>
        /// Maps known exception types to appropriate HTTP status codes
        /// and returns JSON error response.
        /// </summary>
        /// <param name="context">Current HTTP context.</param>
        /// <param name="exception">Captured exception.</param>
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Default response values
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An error occurred while processing your request.";

            // Map specific exception types to HTTP semantics
            switch (exception)
            {
                case ExternalApiException externalApiEx:
                    statusCode = HttpStatusCode.ServiceUnavailable;
                    message = $"External API error ({externalApiEx.ApiName}): {externalApiEx.Message}";
                    break;

                case ArgumentException argEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = argEx.Message;
                    break;

                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = "The requested resource was not found.";
                    break;

                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = "Unauthorized access.";
                    break;

                default:
                    // Keep default values for unhandled exceptions
                    break;
            }

            // Prepare response payload
            var response = new
            {
                statusCode = (int)statusCode,
                message = message,
                timestamp = DateTime.UtcNow
            };

            // Configure HTTP response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            // Serialize response object to JSON using camelCase
            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Write JSON response to HTTP output
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}

