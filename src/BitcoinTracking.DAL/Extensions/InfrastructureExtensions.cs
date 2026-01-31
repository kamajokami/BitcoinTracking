using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BitcoinTracking.DAL.DataContext;
using BitcoinTracking.DAL.Interfaces;
using BitcoinTracking.DAL.Repositories;
using BitcoinTracking.Shared.Constants;


namespace BitcoinTracking.DAL.Extensions
{
    /// <summary>
    /// Extension methods for registering DAL services in DI container
    /// Entry point for connecting all DAL-related dependencies 
    /// (DbContext, repositories, etc.) from web layer.
    /// </summary>
    public static class InfrastructureExtensions
    {
        /// <summary>
        /// Register Data Access Layer services
        /// Registers ApplicationDbContext with SQL Server
        /// - registers ApplicationDbContext
        /// - configures SQL Server as the database provider
        /// - enables transient failure retries
        /// - registers repository implementations
        /// Application configuration (appsettings.json, environment-specific configs, etc.).
        /// Retrieves database connection string.
        /// </summary>
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register DbContext with SQL Server in DI container
            // DbContext with scoped lifetime only for one DbContext instance per HTTP request
            /*
             * UseSqlServer: Reads DB provider
             * ConnectionString from AppConstants
             * RetryOnFailure for transient errors
             * maxRetryDelay: Maximum delay between retry attempts
             */
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString(AppConstants.Database.DefaultConnectionStringName),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null // default list
                    )
                )
            );

            // Register Repositories - one instance per HTTP request (lifetimes match)
            services.AddScoped<IRecordRepository, RecordRepository>();

            return services;
        }
    }
}
