using BitcoinTracking.BAL.DTOs;


namespace BitcoinTracking.BAL.Interfaces.Services
{
    /// <summary>
    /// Service for fetching and calculating Bitcoin rates
    /// Orchestrates calls to CoinDesk API and ČNB API
    /// </summary>
    public interface IBitcoinService
    {
        /// <summary>
        /// Get current Bitcoin rate in CZK
        /// Fetches BTC/EUR from CoinDesk, EUR/CZK from ČNB, and calculates BTC/CZK
        /// </summary>
        Task<BitcoinRateDto> GetCurrentRateAsync();
    }
}
