using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BitcoinTracking.BAL.Interfaces.ExternalApis;
using BitcoinTracking.BAL.Exceptions;


namespace BitcoinTracking.BAL.Services
{
    /// <summary>
    /// Client implementation for Czech National Bank (ČNB) API.
    /// Fetches current EUR to CZK exchange rate
    /// </summary>
    public class CnbApiClient : ICnbApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CnbApiClient> _logger;


        /// <summary>
        /// DI - Initializes a new instance of CnbApiClient class.
        /// </summary>
        /// <param name="httpClient">
        /// HTTP client used to send requests to ČNB API.
        /// </param>
        /// <param name="configuration">
        /// Application configuration containing ČNB API URLs and endpoints.
        /// </param>
        public CnbApiClient(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<CnbApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        /// <summary>
        /// Retrieves current EUR to CZK exchange rate from ČNB API.
        /// </summary>
        /// <returns>
        /// <see cref="decimal"/> representing EUR/CZK exchange rate.
        /// </returns>
        /// <exception cref="ExternalApiException">
        /// Thrown when the ČNB API request fails, returns a non-success status code,
        /// or when the response cannot be parsed correctly.
        /// </exception>
        public async Task<decimal> GetEurCzkRateAsync()
        {
            try
            {
                // Get base API URL and endpoint from configuration.
                // ČNB API returns a daily text file with exchange rates.
                var baseUrl = _configuration["ExternalApis:Cnb:BaseUrl"];
                var endpoint = _configuration["ExternalApis:Cnb:DailyRatesEndpoint"];
                var fullUrl = $"{baseUrl}{endpoint}";

                _logger.LogInformation("Fetching EUR/CZK rate from ČNB API: {Url}", fullUrl);

                // HTTP GET request to the ČNB API
                var response = await _httpClient.GetAsync(fullUrl);

                // Check for unsuccessful HTTP status codes
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogError("ČNB API returned status {StatusCode}: {Content}",
                        response.StatusCode, errorContent);

                    throw new ExternalApiException(
                        "ČNB API",
                        (int)response.StatusCode,
                        $"ČNB API vrátilo chybu: {response.StatusCode}");
                }

                // Read response content as plain text (ČNB returns TXT format, not JSON)
                var textContent = await response.Content.ReadAsStringAsync();

                // Parse EUR rate from text
                var eurRate = ParseEurRateFromCnbText(textContent);

                _logger.LogInformation("Successfully fetched EUR/CZK rate: {Rate}", eurRate);

                return eurRate;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed when calling ČNB API");
                // Network-level issues such as timeouts or DNS failures
                throw new ExternalApiException("ČNB API", "Chyba při volání ČNB API (síťová chyba)", ex);
            }
            catch (ExternalApiException)
            {
                // Re-throw our custom exception without modification
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when calling ČNB API");
                // Catch-all for unexpected runtime errors
                throw new ExternalApiException("ČNB API", "Neočekávaná chyba při volání ČNB API", ex);
            }
        }

        /// <summary>
        /// Parse EUR exchange course from ČNB text format
        /// ČNB format: 31.01.2026 #21
        /// country|currency|quantity|code|course
        /// EMU|euro|1|EUR|25.123
        /// </summary>
        private decimal ParseEurRateFromCnbText(string textContent)
        {
            try
            {
                // Split response text into individual lines
                var lines = textContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                // Find line with EUR
                foreach (var line in lines)
                {
                    if (line.Contains("|EUR|"))
                    {
                        // Split by pipe: country|currency|quantity|code|course
                        var parts = line.Split('|');

                        if (parts.Length >= 5)
                        {
                            // Last part is exchange course
                            var rateString = parts[4].Trim();

                            // Parse decimal value using Czech culture
                            // (decimal separator is a dot in current ČNB format)
                            if (decimal.TryParse(rateString, NumberStyles.Any, CultureInfo.GetCultureInfo("cs-CZ"), out decimal rate))
                            {
                                _logger.LogDebug("Parsed EUR rate: {Rate} from line: {Line}", rate, line);
                                
                                return rate;
                            }
                        }
                    }
                }

                _logger.LogError("EUR rate not found in ČNB response. Content: {Content}", textContent);
                // EUR entry was not found in response
                throw new ExternalApiException("ČNB API", "Kurz EUR nenalezen v odpovědi z ČNB");
            }
            catch (ExternalApiException)
            {
                // Re-throw known parsing errors
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse EUR rate from ČNB response");

                // Handling unexpected parsing errors
                throw new ExternalApiException("ČNB API", "Chyba při parsování kurzu EUR z odpovědi ČNB", ex);
            }
        }
    }
}
