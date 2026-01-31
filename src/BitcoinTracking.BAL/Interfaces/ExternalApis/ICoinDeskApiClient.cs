

namespace BitcoinTracking.BAL.Interfaces.ExternalApis
{
    /// <summary>
    /// Interface for CoinDesk API client
    /// </summary>
    public interface ICoinDeskApiClient
    {
        /// <summary>
        /// Get current Bitcoin price in EUR
        /// </summary>
        /// <returns>BTC/EUR price</returns>
        Task<decimal> GetBtcEurPriceAsync();
    }
}
