

namespace BitcoinTracking.BAL.DTOs
{
    /// <summary>
    /// DTO for live Bitcoin rate data (from CoinDesk API + ČNB API)
    /// Page Live Data for current course
    /// </summary>
    public class BitcoinRateDto
    {
        /// <summary>
        /// Timestamp when the data was fetched
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Bitcoin price in EUR (from CoinDesk API)
        /// </summary>
        public decimal PriceBtcEur { get; set; }

        /// <summary>
        /// EUR to CZK exchange rate (from ČNB API)
        /// </summary>
        public decimal ExchangeRateEurCzk { get; set; }

        /// <summary>
        /// Calculated Bitcoin price in CZK (BTC/EUR * EUR/CZK)
        /// </summary>
        public decimal PriceBtcCzk { get; set; }

        /// <summary>
        /// Source of BTC/EUR price (e.g., "CoinDesk API")
        /// </summary>
        public string Source { get; set; } = "CoinDesk API";
    }
}
