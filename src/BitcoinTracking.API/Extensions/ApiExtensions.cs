using BitcoinTracking.API.Middleware;

namespace BitcoinTracking.API.Extensions
{
    /// <summary>
    /// Extension methods for API configuration
    /// </summary>
    public static class ApiExtensions
    {
        /// <summary>
        /// Add Exception Handling Middleware to HTTP request pipeline
        /// </summary>
        /// <param name="app">Application builder instance.</param>
        /// <returns>The same <see cref="IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        {
            // Register ExceptionHandlingMiddleware into pipeline
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
