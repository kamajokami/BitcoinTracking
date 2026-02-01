using Microsoft.AspNetCore.Mvc;
using BitcoinTracking.BAL.Interfaces.Services;
using BitcoinTracking.BAL.DTOs;


namespace BitcoinTracking.API.Controllers
{
    /// <summary>
    /// API Controller for Bitcoin rate operations.
    /// Provides endpoints for retrieving live Bitcoin exchange rates.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BitcoinController : ControllerBase
    {
        /// <summary>
        /// Business service responsible for Bitcoin-related operations.
        /// </summary>
        private readonly IBitcoinService _bitcoinService;

        /// <summary>
        /// Logger used for tracing API requests and responses.
        /// </summary>
        private readonly ILogger<BitcoinController> _logger;

        /// <summary>
        /// Initializes a new instance of BitcoinController.
        /// </summary>
        /// <param name="bitcoinService">Bitcoin business logic service.</param>
        /// <param name="logger">Logger instance.</param>
        public BitcoinController(IBitcoinService bitcoinService, ILogger<BitcoinController> logger)
        {
            _bitcoinService = bitcoinService ?? throw new ArgumentNullException(nameof(bitcoinService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets current Bitcoin price in CZK.
        /// Fetches BTC/EUR from CoinDesk, EUR/CZK from ČNB, and calculates final BTC/CZK  value.
        /// </summary>
        /// <returns>Current Bitcoin course</returns>
        /// <response code="200">Current Bitcoin exchange rate DTO.</response>
        /// <response code="503">External API is unavailable</response>
        [HttpGet("live")]
        [ProducesResponseType(typeof(BitcoinRateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<BitcoinRateDto>> GetLiveRate()
        {
            // Log start of API request
            _logger.LogInformation("API: Getting live Bitcoin rate");

            // Delegate rate retrieval to business layer
            var rate = await _bitcoinService.GetCurrentRateAsync();

            _logger.LogInformation("API: Successfully fetched Bitcoin rate: {PriceCzk} CZK", rate.PriceBtcCzk);

            return Ok(rate); // Return HTTP 200 with DTO payload
        }
    }
}
