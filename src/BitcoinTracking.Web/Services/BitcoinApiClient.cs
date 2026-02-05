using BitcoinTracking.BAL.DTOs;
using System.Text.Json;

namespace BitcoinTracking.Web.Services
{
    /// <summary>
    /// HTTP Client for communication with BitcoinTracking REST API.
    /// Light HTTP abstraction layer for MVC controllers.
    /// </summary>
    public class BitcoinApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BitcoinApiClient> _logger;

        /// <summary>
        /// Initializes new instance of BitcoinApiClient
        /// </summary>
        /// <param name="httpClient">Preconfigured HttpClient embedded DI</param>
        /// <param name="logger">Logger instance</param>
        public BitcoinApiClient(HttpClient httpClient, ILogger<BitcoinApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        //  Live Bitcoin Rate
        /// <summary>
        /// Gets current live Bitcoin rate (BTC/EUR and BTC/CZK)
        /// </summary>
        public async Task<BitcoinRateDto> GetLiveRateAsync()
        {
            try
            {
                _logger.LogInformation("Calling API: GET /api/bitcoin/live");

                var response = await _httpClient.GetAsync("/api/bitcoin/live");

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<BitcoinRateDto>();

                _logger.LogInformation("Successfully fetched live Bitcoin rate");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling API: GET /api/bitcoin/live");

                return null;
            }
        }


        /// <summary>
        /// Gets all saved Bitcoin records
        /// </summary>
        public async Task<IEnumerable<SavedRecordDto>> GetAllRecordsAsync()
        {
            try
            {
                _logger.LogInformation("Calling API: GET /api/records");

                var response = await _httpClient.GetAsync("/api/records");

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<SavedRecordDto>>();

                _logger.LogInformation("Successfully fetched all records");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling API: GET /api/records");

                return null;
            }
        }


        /// <summary>
        /// Creates new Bitcoin record via API
        /// </summary>
        public async Task<SavedRecordDto> CreateRecordAsync(CreateRecordDto dto)
        {
            _logger.LogInformation("Calling API: POST /api/records");

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsJsonAsync("/api/records", dto);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "API is unreachable");
                throw new InvalidOperationException("Nelze se připojit k API");
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                _logger.LogWarning(
                    "API returned error {StatusCode}: {Error}",
                    response.StatusCode,
                    errorContent
                );

                // Business error (duplicate note, validation, etc.)
                throw new InvalidOperationException(
                    string.IsNullOrWhiteSpace(errorContent)
                        ? "API returned an error"
                        : errorContent
                );
            }

            try
            {
                var result = await response.Content.ReadFromJsonAsync<SavedRecordDto>();

                if (result == null)
                {
                    throw new InvalidOperationException("API returned empty response");
                }

                _logger.LogInformation("Successfully created record with ID {Id}", result?.Id);

                return result;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Invalid JSON response from API");

                throw new InvalidOperationException("Neplatná odpověď ze serveru");
            }
        }


        /// <summary>
        /// Updates note for existing record
        /// </summary>
        public async Task<bool> UpdateNoteAsync(int id, UpdateRecordNoteDto dto)
        {
            try
            {
                _logger.LogInformation("Calling API: PUT /api/records/{Id}", id);

                var response = await _httpClient.PutAsJsonAsync($"/api/records/{id}", dto);

                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully updated note for record {Id}", id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling API: PUT /api/records/{Id}", id);

                return false;
            }
        }


        /// <summary>
        /// Deletes single record by ID
        /// </summary>
        public async Task<bool> DeleteRecordAsync(int id)
        {
            try
            {
                _logger.LogInformation("Calling API: DELETE /api/records/{Id}", id);

                var response = await _httpClient.DeleteAsync($"/api/records/{id}");

                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully deleted record {Id}", id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling API: DELETE /api/records/{Id}", id);

                return false;
            }
        }


        /// <summary>
        /// Deletes multiple records in bulk
        /// </summary>
        public async Task<int> DeleteRecordsAsync(IEnumerable<int> ids)
        {
            try
            {
                _logger.LogInformation("Calling API: DELETE /api/records (bulk)");

                var response = await _httpClient.PostAsJsonAsync("/api/records/delete-bulk", ids);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<BulkDeleteResultDto>();

                _logger.LogInformation("Successfully deleted multiple records");

                return result?.DeletedCount ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling API: DELETE /api/records (bulk)");

                return 0;
            }
        }
    }
}
