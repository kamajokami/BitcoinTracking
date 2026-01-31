using Microsoft.Extensions.Logging;
using BitcoinTracking.BAL.DTOs;
using BitcoinTracking.BAL.Interfaces.Services;
using BitcoinTracking.BAL.Validators;
using BitcoinTracking.DAL.Entities;
using BitcoinTracking.DAL.Interfaces;


namespace BitcoinTracking.BAL.Services
{
    /// <summary>
    /// Service for Bitcoin record business logic
    /// Handles CRUD operations, validation, entity-to-DTO mapping and logging.
    /// </summary>
    public class RecordService : IRecordService
    {
        /// <summary>
        /// Calling DAL via IRecordRepository
        /// </summary>
        private readonly IRecordRepository _recordRepository;

        private readonly ILogger<RecordService> _logger;

        /// <summary>
        /// Initializes new instance ofRecordService
        /// </summary>
        /// <param name="recordRepository">
        /// Repository used for data access operations related to Bitcoin records.
        /// </param>
        public RecordService(
            IRecordRepository recordRepository,
            ILogger<RecordService> logger)
        {
            _recordRepository = recordRepository ?? throw new ArgumentNullException(nameof(recordRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        /// <summary>
        /// Gets all Bitcoin records from the data store.
        /// </summary>
        /// <returns>
        /// Collection of saved Bitcoin records mapped to DTOs.
        /// </returns>
        public async Task<IEnumerable<SavedRecordDto>> GetAllRecordsAsync()
        {
            _logger.LogInformation("Fetching all Bitcoin records");

            var entities = await _recordRepository.GetAllAsync();
            var dtos = entities.Select(MapEntityToDto).ToList();

            _logger.LogInformation("Fetched {Count} records", dtos.Count);
            return dtos;
        }


        /// <summary>
        /// Gets single Bitcoin record by its unique identifier.
        /// </summary>
        /// <param name="id">Record identifier.</param>
        /// <returns>
        /// CorrespondingSavedRecordDto: if found; otherwise null.
        /// </returns>
        public async Task<SavedRecordDto> GetRecordByIdAsync(int id)
        {
            _logger.LogInformation("Fetching Bitcoin record by ID: {Id}", id);

            var entity = await _recordRepository.GetByIdAsync(id);

            if (entity == null)
            {
                _logger.LogWarning("Record with ID {Id} not found", id);
                return null;
            }

            return MapEntityToDto(entity);
        }


        /// <summary>
        /// Retrieves a paged subset of Bitcoin records.
        /// </summary>
        /// <param name="pageNumber">Page number (1-based).</param>
        /// <param name="pageSize">Number of records per page.</param>
        /// <returns>Collection of paged Bitcoin records mapped to DTOs.</returns>
        public async Task<IEnumerable<SavedRecordDto>> GetPagedRecordsAsync(int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching paged records: Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);

            var entities = await _recordRepository.GetPagedAsync(pageNumber, pageSize);
            var dtos = entities.Select(MapEntityToDto).ToList();

            return dtos;
        }


        /// <summary>
        /// Gets the total count of Bitcoin records stored in the system.
        /// </summary>
        /// <returns>Total number of records.</returns>
        public async Task<int> GetRecordCountAsync()
        {
            var count = await _recordRepository.GetCountAsync();
            _logger.LogInformation("Total record count: {Count}", count);
            return count;
        }


        /// <summary>
        /// Creates a new Bitcoin record after validating input data.
        /// </summary>
        /// <param name="dto">DTO containing data for new record.</param>
        /// <returns>Newly created Bitcoin record mapped to SavedRecordDto.
        /// </returns>
        /// <exception> ArgumentException: Thrown when validation of input data fails.</exception>
        public async Task<SavedRecordDto> CreateRecordAsync(CreateRecordDto dto)
        {
            _logger.LogInformation("Creating new Bitcoin record");

            // Validate DTO
            var validator = new CreateRecordDtoValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed for CreateRecordDto: {Errors}", errors);
                throw new ArgumentException($"Validace selhala: {errors}");
            }

            // Map DTO to Entity
            var entity = new BitcoinRecord
            {
                Timestamp = DateTime.Now,
                PriceBtcEur = dto.PriceBtcEur,
                ExchangeRateEurCzk = dto.ExchangeRateEurCzk,
                PriceBtcCzk = dto.PriceBtcCzk,
                Note = dto.Note
            };

            // Save to database
            var savedEntity = await _recordRepository.AddAsync(entity);

            _logger.LogInformation("Created Bitcoin record with ID: {Id}", savedEntity.Id);

            return MapEntityToDto(savedEntity);
        }


        /// <summary>
        /// Updates note field of an existing Bitcoin record.
        /// </summary>
        /// <param name="dto">DTO containing record ID and new note value.</param>
        /// <returns>True if the record was updated successfully; otherwise false.</returns>
        /// <exception> ArgumentException: Thrown when validation of input data fails.</exception>
        public async Task<bool> UpdateNoteAsync(UpdateRecordNoteDto dto)
        {
            _logger.LogInformation("Updating note for record ID: {Id}", dto.Id);

            // Validate DTO
            var validator = new UpdateNoteDtoValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));

                _logger.LogWarning("Validation failed for UpdateNoteDto: {Errors}", errors);

                throw new ArgumentException($"Validace selhala: {errors}");
            }

            var result = await _recordRepository.UpdateNoteAsync(dto.Id, dto.Note);

            if (result)
            {
                _logger.LogInformation("Successfully updated note for record ID: {Id}", dto.Id);
            }
            else
            {
                _logger.LogWarning("Failed to update note for record ID: {Id} - record not found", dto.Id);
            }

            return result;
        }


        /// <summary>
        /// Deletes Bitcoin record by its identifier.
        /// </summary>
        /// <param name="id">Record identifier.</param>
        /// <returns>True if record was deleted; otherwise false.</returns>
        public async Task<bool> DeleteRecordAsync(int id)
        {
            _logger.LogInformation("Deleting Bitcoin record with ID: {Id}", id);

            var result = await _recordRepository.DeleteAsync(id);

            if (result)
            {
                _logger.LogInformation("Successfully deleted record ID: {Id}", id);
            }
            else
            {
                _logger.LogWarning("Failed to delete record ID: {Id} - record not found", id);
            }

            return result;
        }


        /// <summary>
        /// Deletes multiple Bitcoin records in a single operation.
        /// </summary>
        /// <param name="ids">Collection of record identifiers to delete.</param>
        /// <returns>Number of records that were successfully deleted.</returns>
        public async Task<int> DeleteRecordsAsync(IEnumerable<int> ids)
        {
            _logger.LogInformation("Deleting multiple Bitcoin records: {Count} IDs", ids.Count());

            var deletedCount = await _recordRepository.DeleteRangeAsync(ids);

            _logger.LogInformation("Successfully deleted {Count} records", deletedCount);

            return deletedCount;
        }

        /// <summary>
        /// Map BitcoinRecord entity to SavedRecordDto
        /// </summary>
        /// <param name="entity">Bitcoin record entity from data layer.</param>
        /// <returns>Mapped DTO representation of the record.</returns>
        private SavedRecordDto MapEntityToDto(BitcoinRecord entity)
        {
            return new SavedRecordDto
            {
                Id = entity.Id,
                Timestamp = entity.Timestamp,
                PriceBtcEur = entity.PriceBtcEur,
                ExchangeRateEurCzk = entity.ExchangeRateEurCzk,
                PriceBtcCzk = entity.PriceBtcCzk,
                Note = entity.Note
            };
        }
    }
}
