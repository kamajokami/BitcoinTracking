namespace BitcoinTracking.DAL.Entities
{
    /// <summary>
    /// Entity representing a saved one Bitcoin price record
    /// </summary>
    public class BitcoinRecord
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Čas získání
        /// Timestamp when the data was fetched
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Cena BTC/EUR
        /// Bitcoin price in EUR (from CoinDesk API)
        /// </summary>
        public decimal PriceBtcEur { get; set; }

        /// <summary>
        /// Kurz EUR/CZK
        /// EUR to CZK exchange rate (from ČNB API)
        /// </summary>
        public decimal ExchangeRateEurCzk { get; set; }

        /// <summary>
        /// Vypočtená cena BTC/CZK
        /// Calculated Bitcoin price in CZK (BTC/EUR * EUR/CZK)
        /// </summary>
        public decimal PriceBtcCzk { get; set; }

        /// <summary>
        /// Poznámka (editovatelná)
        /// User's note (editable field - REQUIRED in saved data)
        /// Note field must be filled in before saving
        /// </summary>
        public string Note { get; set; } // ← Required ve fluent api
    }
}
