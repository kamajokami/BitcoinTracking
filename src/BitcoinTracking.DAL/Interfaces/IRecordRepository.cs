using BitcoinTracking.DAL.Entities;

namespace BitcoinTracking.DAL.Interfaces
{
    /// <summary>
    /// Repository interface for BitcoinRecord operations
    /// </summary>
    public interface IRecordRepository
    {
        /// <summary>
        /// Get all Bitcoin records
        /// </summary>
        Task<IEnumerable<BitcoinRecord>> GetAllAsync();

        /// <summary>
        /// Get Bitcoin record by ID
        /// </summary>
        Task<BitcoinRecord> GetByIdAsync(int id);

        /// <summary>
        /// Get records with pagination
        /// </summary>
        Task<IEnumerable<BitcoinRecord>> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Get total count of records
        /// </summary>
        Task<int> GetCountAsync();

        /// <summary>
        /// Add new Bitcoin record
        /// </summary>
        Task<BitcoinRecord> AddAsync(BitcoinRecord record);

        /// <summary>
        /// Update existing Bitcoin record
        /// </summary>
        Task<BitcoinRecord> UpdateAsync(BitcoinRecord record);

        /// <summary>
        /// Update only the Note field (for editing in Saved Data page)
        /// </summary>
        Task<bool> UpdateNoteAsync(int id, string note);

        /// <summary>
        /// Delete Bitcoin record by ID
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Delete multiple records by IDs
        /// </summary>
        Task<int> DeleteRangeAsync(IEnumerable<int> ids);

        /// <summary>
        /// Utility
        /// Check if record exists
        /// </summary>
        Task<bool> ExistsAsync(int id);
    }
}
