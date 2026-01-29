using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using BitcoinTracking.Shared.Constants;

namespace BitcoinTracking.Shared.Logging
{
    /// <summary>
    /// Centralized Serilog configuration for the entire application
    /// </summary>
    public static class SerilogConfiguration
    {
        /// <summary>
        /// Configures Serilog with file and console sinks
        /// </summary>
        public static ILogger CreateLogger(IConfiguration configuration)
        {
            var logDirectory = Path.Combine(AppContext.BaseDirectory, AppConstants.Logging.LogDirectory);

            // Ensure log directory exists
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            var logFilePath = Path.Combine(logDirectory, AppConstants.Logging.LogFileName);

            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "BitcoinTracking")
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: logFilePath,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 30)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
