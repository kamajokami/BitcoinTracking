

namespace BitcoinTracking.BAL.Interfaces.ExternalApis
{
    /// <summary>
    /// Interface for ČNB (Czech National Bank) API client
    /// </summary>
    public interface ICnbApiClient
    {
        /// <summary>
        /// Get current EUR to CZK exchange rate
        /// </summary>
        /// <returns>EUR/CZK exchange rate</returns>
        Task<decimal> GetEurCzkRateAsync();
    }
}
