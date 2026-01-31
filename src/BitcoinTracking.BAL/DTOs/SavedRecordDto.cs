

namespace BitcoinTracking.BAL.DTOs
{
    /// <summary>
    /// DTO for displaying saved Bitcoin records
    /// Page Saved Data
    /// </summary>
    public class SavedRecordDto
    {
        /// <summary>
        /// Record ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Timestamp when the data was saved
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Bitcoin price in EUR
        /// </summary>
        public decimal PriceBtcEur { get; set; }

        /// <summary>
        /// EUR to CZK exchange rate
        /// </summary>
        public decimal ExchangeRateEurCzk { get; set; }

        /// <summary>
        /// Calculated Bitcoin price in CZK
        /// </summary>
        public decimal PriceBtcCzk { get; set; }

        /// <summary>
        /// User's editable note
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Formatted timestamp for display (DD.MM.YYYY HH:mm)
        /// </summary>
        public string FormattedTimestamp => Timestamp.ToString("dd.MM.yyyy HH:mm");
    }
}
