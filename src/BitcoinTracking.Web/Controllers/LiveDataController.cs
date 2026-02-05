using BitcoinTracking.BAL.DTOs;
using BitcoinTracking.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BitcoinTracking.Web.Controllers
{
    /// <summary>
    /// MVC controller responsible for displaying and handling live Bitcoin rate data.
    /// Thin layer between UI (JavaScript) and REST API.
    /// </summary>
    public class LiveDataController : Controller
    {
        private readonly BitcoinApiClient _apiClient;
        private readonly ILogger<LiveDataController> _logger;

        /// <summary>
        /// Initializes new instance of LiveDataController
        /// </summary>
        /// <param name="apiClient">Client used to communicate with Bitcoin REST API</param>
        /// <param name="logger">Logger instance for diagnostic logging</param>
        public LiveDataController(BitcoinApiClient apiClient, ILogger<LiveDataController> logger)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Display Live Data page
        /// Actual data are loaded asynchronously via AJAX
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Gets current live Bitcoin exchange rate
        /// AJAX endpoint to be called via AJAX from UI: Get live Bitcoin rate
        /// </summary>
        /// <returns>
        /// JSON response containing live rate data or an error message.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetLiveRate()
        {
            _logger.LogInformation("Web: Requesting live Bitcoin rate");

            try
            {
                var rate = await _apiClient.GetLiveRateAsync();

                if (rate == null)
                {
                    _logger.LogWarning("Web: Failed to retrieve live Bitcoin. Rate returned null.");

                    return Json(new
                    {
                        success = false,
                        message = "Nepodařilo se načíst aktuální kurz"
                    });
                }

                return Json(new
                {
                    success = true,
                    data = rate
                });
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Web: Exception while retrieving live Bitcoin rate");

                return Json(new
                {
                    success = false,
                    message = "Došlo k chybě při načítání dat"
                });
            }
        }

        /// <summary>
        /// AJAX endpoint: Saves current Bitcoin rate into database.
        /// Expects data sent from client as JSON.
        /// </summary>
        /// <param name="createRecordDto">DTO containing rate values and user note.</param>
        /// <returns>
        /// JSON response indicating success or failure.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRate([FromBody] CreateRecordDto createRecordDto)
        {
            if (createRecordDto == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Neplatná data"
                });
            }

            // Required note
            if (string.IsNullOrWhiteSpace(createRecordDto.Note))
            {
                return Json(new
                {
                    success = false,
                    message = "Poznámka je povinná"
                });
            }

            try
            {
                _logger.LogInformation("Web: Saving Bitcoin rate to database");

                var result = await _apiClient.CreateRecordAsync(createRecordDto);

                return Json(new
                {
                    success = true,
                    message = "Záznam byl úspěšně uložen",
                    data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                // business / validation error (duplicate, etc.)
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Web: Exception while saving Bitcoin rate");

                return Json(new
                {
                    success = false,
                    message = "Došlo k neočekávané chybě při ukládání záznamu"
                });
            }
        }
    }
}
