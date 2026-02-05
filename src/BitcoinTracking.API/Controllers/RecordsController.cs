using Microsoft.AspNetCore.Mvc;
using BitcoinTracking.BAL.Interfaces.Services;
using BitcoinTracking.BAL.DTOs;

namespace BitcoinTracking.API.Controllers
{
    /// <summary>
    /// API Controller for saved Bitcoin records operations.
    /// CRUD operations for stored Bitcoin price records.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RecordsController : ControllerBase
    {
        /// <summary>
        /// Business service responsible for record operations.
        /// </summary>
        private readonly IRecordService _recordService;
        private readonly ILogger<RecordsController> _logger;

        /// <summary>
        /// Initializes new instance RecordsController.
        /// </summary>
        /// <param name="recordService">Record business service.</param>
        /// <param name="logger">Logger instance.</param>
        public RecordsController(IRecordService recordService, ILogger<RecordsController> logger)
        {
            _recordService = recordService ?? throw new ArgumentNullException(nameof(recordService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all saved Bitcoin records
        /// </summary>
        /// <returns>Collection of saved Bitcoin records.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SavedRecordDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SavedRecordDto>>> GetAll()
        {
            _logger.LogInformation("API: Getting all Bitcoin records");

            var records = await _recordService.GetAllRecordsAsync();

            _logger.LogInformation("API: Returning {Count} records", records.Count());

            return Ok(records);
        }

        /// <summary>
        /// Gets a specific Bitcoin record by its identifier.
        /// </summary>
        /// <param name="id">Record identifier.</param>
        /// <returns>Saved Bitcoin record.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SavedRecordDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SavedRecordDto>> GetById(int id)
        {
            _logger.LogInformation("API: Getting Bitcoin record by ID: {Id}", id);

            var record = await _recordService.GetRecordByIdAsync(id);

            if (record == null)
            {
                _logger.LogWarning("API: Record with ID {Id} not found", id);
                return NotFound(new { message = $"Record with ID {id} not found" });
            }

            return Ok(record);
        }

        /// <summary>
        /// Creates new Bitcoin record.
        /// </summary>
        /// <param name="dto">DTO containing record data.</param>
        /// <returns>Created Bitcoin record.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(SavedRecordDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SavedRecordDto>> Create([FromBody] CreateRecordDto dto)
        {
            _logger.LogInformation("API: Creating new Bitcoin record");

            try
            {
                var createdRecord = await _recordService.CreateRecordAsync(dto);

                _logger.LogInformation("API: Created record with ID: {Id}", createdRecord.Id);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdRecord.Id },
                    createdRecord);
            }
            catch (InvalidOperationException ex) 
            {
                // BUSINESS PROBLEM (duplicate note)
                _logger.LogWarning(ex, "API: Business rule violation");

                return BadRequest(new
                {
                    error = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                // Validate error DTO
                _logger.LogWarning(ex, "API: Validation error");

                return BadRequest(new
                {
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API: Unexpected error while creating record");

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "Došlo k neočekávané chybě"
                });
            }
        }

        /// <summary>
        /// Updates note of an existing Bitcoin record.
        /// </summary>
        /// <param name="id">Record identifier from URL.</param>
        /// <param name="dto">DTO containing updated note.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] UpdateRecordNoteDto dto)
        {
            _logger.LogInformation("API: Updating note for record ID: {Id}", id);

            // Validate ID consistency
            if (id != dto.Id)
            {
                _logger.LogWarning("API: ID mismatch - URL: {UrlId}, Body: {BodyId}", id, dto.Id);

                return BadRequest(new { message = "ID in URL does not match ID in body" });
            }

            var success = await _recordService.UpdateNoteAsync(dto);

            if (!success)
            {
                _logger.LogWarning("API: Failed to update note for record ID: {Id}", id);

                return NotFound(new { message = $"Record with ID {id} not found" });
            }

            _logger.LogInformation("API: Successfully updated note for record ID: {Id}", id);

            return NoContent();
        }

        /// <summary>
        /// Deletes a Bitcoin record by ID.
        /// </summary>
        /// <param name="id">Record identifier.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("API: Deleting Bitcoin record with ID: {Id}", id);

            var success = await _recordService.DeleteRecordAsync(id);

            if (!success)
            {
                _logger.LogWarning("API: Failed to delete record ID: {Id} - not found", id);
                return NotFound(new { message = $"Record with ID {id} not found" });
            }

            _logger.LogInformation("API: Successfully deleted record ID: {Id}", id);

            return NoContent();
        }

        /// <summary>
        /// Deletes multiple Bitcoin records at once.
        /// </summary>
        /// <param name="ids">Collection of record IDs.</param>
        [HttpDelete]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteMultiple([FromBody] IEnumerable<int> ids)
        {
            _logger.LogInformation("API: Deleting multiple Bitcoin records: {Count} IDs", ids.Count());

            if (ids == null || !ids.Any())
            {
                _logger.LogWarning("API: No IDs provided for deletion");
                return BadRequest(new { message = "No IDs provided" });
            }

            var deletedCount = await _recordService.DeleteRecordsAsync(ids);

            _logger.LogInformation("API: Successfully deleted {Count} records", deletedCount);

            return Ok(new { deletedCount = deletedCount, message = $"Deleted {deletedCount} record(s)" });
        }
    }
}
