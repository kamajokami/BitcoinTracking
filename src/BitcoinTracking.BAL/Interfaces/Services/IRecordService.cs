using BitcoinTracking.BAL.DTOs;


namespace BitcoinTracking.BAL.Interfaces.Services
{
    /// <summary>
    /// Service for Bitcoin record business logic
    /// </summary>
    public interface IRecordService
    {
        /// <summary>
        /// Get all saved Bitcoin records
        /// </summary>
        Task<IEnumerable<SavedRecordDto>> GetAllRecordsAsync();

        /// <summary>
        /// Get saved record by ID
        /// </summary>
        Task<SavedRecordDto> GetRecordByIdAsync(int id);

        /// <summary>
        /// Get records with pagination
        /// </summary>
        Task<IEnumerable<SavedRecordDto>> GetPagedRecordsAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Get total count of records
        /// </summary>
        Task<int> GetRecordCountAsync();

        /// <summary>
        /// Create new Bitcoin record
        /// Validates DTO before saving
        /// </summary>
        Task<SavedRecordDto> CreateRecordAsync(CreateRecordDto dto);

        /// <summary>
        /// Update note of existing record
        /// Validates note before updating
        /// </summary>
        Task<bool> UpdateNoteAsync(UpdateRecordNoteDto dto);

        /// <summary>
        /// Delete record by ID
        /// </summary>
        Task<bool> DeleteRecordAsync(int id);

        /// <summary>
        /// Delete multiple records by IDs
        /// </summary>
        Task<int> DeleteRecordsAsync(IEnumerable<int> ids);
    }
}
