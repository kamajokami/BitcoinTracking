

namespace BitcoinTracking.BAL.DTOs
{
    /// <summary>
    /// DTO for creating a new Bitcoin record
    /// "Save Data" button on Live Data page
    /// </summary>
    public class CreateRecordDto
    {
        /// <summary>
        /// Bitcoin price in EUR (from CoinDesk API)
        /// </summary>
        public decimal PriceBtcEur { get; set; }

        /// <summary>
        /// EUR to CZK exchange rate (from ČNB API)
        /// </summary>
        public decimal ExchangeRateEurCzk { get; set; }

        /// <summary>
        /// Calculated Bitcoin price in CZK
        /// </summary>
        public decimal PriceBtcCzk { get; set; }

        /// <summary>
        /// User's note (REQUIRED by assignment)
        /// </summary>
        public string Note { get; set; }
    }
}
