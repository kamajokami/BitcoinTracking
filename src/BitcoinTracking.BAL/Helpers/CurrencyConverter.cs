

namespace BitcoinTracking.BAL.Helpers
{
    /// <summary>
    /// Helper for currency conversions and calculations.
    /// Contains pure calculation methods without external dependencies.
    /// </summary>
    public static class CurrencyConverter
    {
        /// <summary>
        /// Converts BTC/EUR to BTC/CZK using EUR/CZK exchange rate
        /// Formula: BTC/CZK = BTC/EUR * EUR/CZK
        /// </summary>
        /// <param name="btcEurPrice">Bitcoin price in EUR</param>
        /// <param name="eurCzkRate">EUR to CZK exchange rate</param>
        /// <returns>Bitcoin price in CZK</returns>
        public static decimal ConvertBtcEurToCzk(decimal btcEurPrice, decimal eurCzkRate)
        {
            if (btcEurPrice <= 0)
                throw new ArgumentException("BTC/EUR price must be greater than zero", nameof(btcEurPrice));

            if (eurCzkRate <= 0)
                throw new ArgumentException("EUR/CZK rate must be greater than zero", nameof(eurCzkRate));

            return btcEurPrice * eurCzkRate;
        }

        /// <summary>
        /// Rounds currency value to specified decimal places
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <param name="decimals">Number of decimal places (default: 2)</param>
        /// <returns>Rounded value</returns>
        public static decimal RoundCurrency(decimal value, int decimals = 2)
        {
            return Math.Round(value, decimals, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Formats decimal value as currency string with CZK
        /// </summary>
        /// <param name="value">Value to format</param>
        /// <returns>Formatted string (e.g., "1 234 567,89 CZK")</returns>
        public static string FormatAsCzk(decimal value)
        {
            return $"{value:N2} CZK";
        }

        /// <summary>
        /// Formats decimal value as currency string with EUR
        /// </summary>
        /// <param name="value">Value to format</param>
        /// <returns>Formatted string (e.g., "12 345,67 EUR")</returns>
        public static string FormatAsEur(decimal value)
        {
            return $"{value:N2} EUR";
        }
    }
}
