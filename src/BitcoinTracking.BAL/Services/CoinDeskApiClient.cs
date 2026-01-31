using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BitcoinTracking.BAL.Interfaces.ExternalApis;
using BitcoinTracking.BAL.Exceptions;


namespace BitcoinTracking.BAL.Services
{
    /// <summary>
    /// Client implementation for the CoinDesk API.
    /// Responsible for fetching the current Bitcoin price in EUR.
    /// </summary>
    public class CoinDeskApiClient : ICoinDeskApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CoinDeskApiClient> _logger;


        /// <summary>
        /// DI - Initializes a new instance of CoinDeskApiClient class.
        /// </summary>
        /// <param name="httpClient">
        /// HTTP client used to send requests to the CoinDesk API.
        /// </param>
        /// <param name="configuration">
        /// Application configuration used to retrieve API URLs and endpoints.
        /// </param>
        public CoinDeskApiClient(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<CoinDeskApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        /// <summary>
        /// Retrieves current Bitcoin price in EUR from CoinDesk API.
        /// </summary>
        /// <returns>
        /// A <see cref="decimal"/> representing current BTC/EUR price.
        /// </returns>
        /// <exception cref="ExternalApiException">
        /// Thrown when CoinDesk API request fails, returns a non-success status code,
        /// or when response cannot be parsed correctly.
        /// </exception>
        public async Task<decimal> GetBtcEurPriceAsync()
        {
            try
            {
                // Get API URL from configuration
                // Retrieve base URL and endpoint path from configuration
                // Modification without changing code
                var baseUrl = _configuration["ExternalApis:CoinDesk:BaseUrl"];
                var endpoint = _configuration["ExternalApis:CoinDesk:BtcEurEndpoint"];
                var fullUrl = $"{baseUrl}{endpoint}";

                _logger.LogInformation("Fetching BTC/EUR price from CoinDesk API: {Url}", fullUrl);

                // Make HTTP GET request to CoinDesk API
                var response = await _httpClient.GetAsync(fullUrl);

                // Check for non-success HTTP status codes
                if (!response.IsSuccessStatusCode)
                {
                    // Read response body for logging and diagnostics
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogError("CoinDesk API returned status {StatusCode}: {Content}",
                        response.StatusCode, errorContent);

                    // Custom exception: Wrap external API failure
                    throw new ExternalApiException(
                        "CoinDesk API",
                        (int)response.StatusCode,
                        $"CoinDesk API vrátilo chybu: {response.StatusCode}");
                }

                // Read response body as string
                var jsonString = await response.Content.ReadAsStringAsync();

                // Parse JSON response
                var jsonDocument = JsonDocument.Parse(jsonString);

                // Navigate through JSON structure to price value: data.BTC-EUR.price
                var price = jsonDocument.RootElement
                    .GetProperty("data")
                    .GetProperty("BTC-EUR")
                    .GetProperty("price")
                    .GetDecimal();

                _logger.LogInformation("Successfully fetched BTC/EUR price: {Price}", price);

                return price;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed when calling CoinDesk API");
                // Network-level errors (DNS issues, timeouts, connection failures)
                throw new ExternalApiException("CoinDesk API", "Chyba při volání CoinDesk API (síťová chyba)", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse CoinDesk API response");
                // Thrown when JSON parsing fails
                throw new ExternalApiException("CoinDesk API", "Chyba při parsování odpovědi z CoinDesk API", ex);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Expected JSON property not found in CoinDesk API response");
                // Thrown when expected JSON properties are missing
                throw new ExternalApiException("CoinDesk API", "Neočekávaný formát odpovědi z CoinDesk API", ex);
            }
            catch (ExternalApiException)
            {
                // Re-throw our custom exception without modification
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when calling CoinDesk API");
                // Catch-all for any unexpected errors
                throw new ExternalApiException("CoinDesk API", "Neočekávaná chyba při volání CoinDesk API", ex);
            }
        }
    }
}
