using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BitcoinTracking.BAL.Interfaces.Services;
using BitcoinTracking.BAL.Interfaces.ExternalApis;
using BitcoinTracking.BAL.Services;
using BitcoinTracking.BAL.Validators;
using FluentValidation;


namespace BitcoinTracking.BAL.Extensions
{
    /// <summary>
    /// Extension methods for registering Business Logic Layer (BAL) services
    /// into the dependency injection container.
    /// </summary>
    public static class BusinessLogicExtensions
    {
        /// <summary>
        /// Registers all Business Logic Layer services, external API clients,
        /// and validators required by the application.
        /// </summary>
        /// <param name="services">Service collection to which BAL services will be registered.</param>
        /// <param name="configuration">Application configuration used to read external API settings.</param>
        /// <returns>The same IServiceCollection instance to allow fluent chaining.
        /// </returns>
        /// <exception>
        /// InvalidOperationException: Thrown when required external API configuration values are missing.
        /// </exception>
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services, IConfiguration configuration)
        {
            // External API Clients (with HttpClientFactory)

            /// <summary>
            /// Registers CoinDesk API HTTP client.
            /// Used for retrieving live Bitcoin price data.
            /// </summary>
            services.AddHttpClient<ICoinDeskApiClient, CoinDeskApiClient>(client =>
            {
                // Read base URL from configuration and fail fast if missing
                var baseUrl = configuration["ExternalApis:CoinDesk:BaseUrl"] 
                ?? throw new InvalidOperationException("CoinDesk BaseUrl is not configured");

                // Set base address for all outgoing requests
                client.BaseAddress = new Uri(baseUrl);

                // Define reasonable timeout to avoid hanging requests
                client.Timeout = TimeSpan.FromSeconds(30);

                // Accept JSON responses from API
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });


            /// <summary>
            /// Registers Czech National Bank (ČNB) API HTTP client.
            /// Used for retrieving EUR to CZK exchange rates.
            /// </summary>
            services.AddHttpClient<ICnbApiClient, CnbApiClient>(client =>
            {
                // Read base URL from configuration and fail fast if missing
                var baseUrl = configuration["ExternalApis:Cnb:BaseUrl"] 
                ?? throw new InvalidOperationException("Cnb BaseUrl is not configured");

                // Set base address for all outgoing requests
                client.BaseAddress = new Uri(baseUrl);

                // Define reasonable timeout to avoid hanging requests
                client.Timeout = TimeSpan.FromSeconds(30);

                // ČNB API typically returns plain text responses
                client.DefaultRequestHeaders.Add("Accept", "text/plain");
            });

            // Business Logic Services

            /// <summary>
            /// Registers Bitcoin-related business logic service.
            /// Handles aggregation of external API data and domain logic.
            /// </summary>
            services.AddScoped<IBitcoinService, BitcoinService>();

            /// Registers record management service.
            /// Handles CRUD operations, validation, and mapping for Bitcoin price records.
            services.AddScoped<IRecordService, RecordService>();

            // FluentValidation Validators

            /// <summary>
            /// Registers all FluentValidation validators located in the BAL assembly.
            /// </summary>
            services.AddValidatorsFromAssemblyContaining<CreateRecordDtoValidator>();

            return services;
        }
    }
}
