using Microsoft.Extensions.Logging;
using BitcoinTracking.BAL.DTOs;
using BitcoinTracking.BAL.Interfaces.Services;
using BitcoinTracking.BAL.Interfaces.ExternalApis;


namespace BitcoinTracking.BAL.Services
{
    /// <summary>
    /// Service for Bitcoin rate operations
    /// Orchestrates CoinDesk API (BTC/EUR) and ČNB API (EUR/CZK) to calculate BTC/CZK
    /// </summary>
    public class BitcoinService : IBitcoinService
    {
        private readonly ICoinDeskApiClient _coinDeskClient;
        private readonly ICnbApiClient _cnbClient;
        private readonly ILogger<BitcoinService> _logger;

        public BitcoinService(
            ICoinDeskApiClient coinDeskClient,
            ICnbApiClient cnbClient,
            ILogger<BitcoinService> logger)
        {
            _coinDeskClient = coinDeskClient ?? throw new ArgumentNullException(nameof(coinDeskClient));
            _cnbClient = cnbClient ?? throw new ArgumentNullException(nameof(cnbClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<BitcoinRateDto> GetCurrentRateAsync()
        {
            _logger.LogInformation("Starting to fetch current Bitcoin rate");

            try
            {
                // Step 1: Fetch BTC/EUR price from CoinDesk API
                _logger.LogDebug("Fetching BTC/EUR price from CoinDesk API");

                var btcEurPrice = await _coinDeskClient.GetBtcEurPriceAsync();

                _logger.LogInformation("BTC/EUR price: {Price}", btcEurPrice);

                // Step 2: Fetch EUR/CZK exchange rate from ČNB API
                _logger.LogDebug("Fetching EUR/CZK rate from ČNB API");

                var eurCzkRate = await _cnbClient.GetEurCzkRateAsync();

                _logger.LogInformation("EUR/CZK rate: {Rate}", eurCzkRate);

                // Step 3: Calculate BTC/CZK price
                var btcCzkPrice = CalculateBtcCzkPrice(btcEurPrice, eurCzkRate);

                _logger.LogInformation("Calculated BTC/CZK price: {Price}", btcCzkPrice);

                // Step 4: Create and return DTO
                var result = new BitcoinRateDto
                {
                    Timestamp = DateTime.Now,
                    PriceBtcEur = Math.Round(btcEurPrice, 2),
                    ExchangeRateEurCzk = Math.Round(eurCzkRate, 4),
                    PriceBtcCzk = Math.Round(btcCzkPrice, 2),
                    Source = "CoinDesk API + ČNB API"
                };

                _logger.LogInformation("Successfully fetched current Bitcoin rate");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching current Bitcoin rate");

                throw;
            }
        }

        /// <summary>
        /// Calculate BTC/CZK price from BTC/EUR and EUR/CZK
        /// Formula: BTC/CZK = BTC/EUR * EUR/CZK
        /// </summary>
        private decimal CalculateBtcCzkPrice(decimal btcEurPrice, decimal eurCzkRate)
        {
            if (btcEurPrice <= 0)
                throw new ArgumentException("BTC/EUR price must be greater than 0", nameof(btcEurPrice));

            if (eurCzkRate <= 0)
                throw new ArgumentException("EUR/CZK rate must be greater than 0", nameof(eurCzkRate));

            return btcEurPrice * eurCzkRate;
        }
    }
}
