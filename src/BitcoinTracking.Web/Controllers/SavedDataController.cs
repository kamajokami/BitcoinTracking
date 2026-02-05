using BitcoinTracking.BAL.DTOs;
using BitcoinTracking.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BitcoinTracking.Web.Controllers
{
    /// <summary>
    /// Controller responsible for displaying and managing saved Bitcoin rate records.
    /// </summary>
    public class SavedDataController : Controller
    {
        private readonly BitcoinApiClient _apiClient;
        private readonly ILogger<SavedDataController> _logger;

        /// <summary>
        /// Initializes a new instance ofSavedDataController
        /// </summary>
        public SavedDataController(BitcoinApiClient apiClient, ILogger<SavedDataController> logger)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Display Saved Data page
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// AJAX endpoint: Gets all saved Bitcoin rate records
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllRecords()
        {
            try
            {
                _logger.LogInformation("Web: Getting all saved records");

                var records = await _apiClient.GetAllRecordsAsync();

                if (records == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Nepodařilo se načíst záznamy"
                    });
                }

                return Json(new
                {
                    success = true,
                    data = records
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all records");

                return Json(new
                {
                    success = false,
                    message = "Chyba při načítání záznamů: " + ex.Message
                });
            }
        }


        /// <summary>
        /// AJAX endpoint: Updates note for a specific record
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateNote([FromBody] UpdateRecordNoteDto updateRecordNoteDto)
        {

            try
            {
                if (updateRecordNoteDto == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Neplatná data požadavku"
                    });
                }

                _logger.LogInformation("Web: Updating note for record {Id}", updateRecordNoteDto.Id);

                if (string.IsNullOrWhiteSpace(updateRecordNoteDto.Note))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Poznámka je povinná"
                    });
                }

                var result = await _apiClient.UpdateNoteAsync(updateRecordNoteDto.Id, updateRecordNoteDto);

                return Json(new
                {
                    success = result,
                    message = result ? "Poznámka aktualizována" : "Nepodařilo se aktualizovat poznámku"
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating note for record {Id}", updateRecordNoteDto?.Id);

                return Json(new
                {
                    success = false,
                    message = "Chyba při aktualizaci poznámky: " + ex.Message
                });
            }
        }


        /// <summary>
        /// AJAX endpoint: Deletes a single saved record
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRecord([FromBody] DeleteRecordDto deleteRecordDto)
        {
            try
            {
                if (deleteRecordDto == null || deleteRecordDto.Id <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Neplatné ID záznamu"
                    });
                }

                _logger.LogInformation("Web: Deleting record {Id}", deleteRecordDto.Id);

                var result = await _apiClient.DeleteRecordAsync(deleteRecordDto.Id);

                return Json(new
                {
                    success = result,
                    message = result ? "Záznam smazán" : "Nepodařilo se smazat záznam"
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting record {Id}", deleteRecordDto?.Id);

                return Json(new
                {
                    success = false,
                    message = "Chyba při mazání záznamu: " + ex.Message
                });
            }
        }


        /// <summary>
        /// AJAX endpoint: Deletes multiple selected records
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSelected([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Žádné záznamy nebyly vybrány"
                    });
                }

                _logger.LogInformation("Web: Deleting {Count} records", ids.Count);

                int deletedCount = 0;

                foreach (var id in ids)
                {
                    if (id <= 0)
                        continue;

                    if (await _apiClient.DeleteRecordAsync(id))
                        deletedCount++;
                }

                return Json(new
                {
                    success = true,
                    deletedCount = deletedCount,
                    message = $"Smazáno {deletedCount} z {ids.Count} záznamů"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting multiple records");
                
                return Json(new
                {
                    success = false,
                    message = "Chyba při mazání záznamů: " + ex.Message
                });
            }
        }
    }
}
