using Microsoft.EntityFrameworkCore;
using BitcoinTracking.DAL.DataContext;
using BitcoinTracking.DAL.Entities;
using BitcoinTracking.DAL.Interfaces;

namespace BitcoinTracking.DAL.Repositories
{
    /// <summary>
    /// Repository implementation responsible for CRUD operations
    /// over the BitcoinRecord entity.
    /// </summary>
    public class RecordRepository : IRecordRepository
    {
        /// <summary>
        /// EF Core database context.
        /// Provides access to DbSet&lt;BitcoinRecord&gt; and database operations.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// Injected ApplicationDbContext instance provided by DI container.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the context is null.
        /// </exception>
        public RecordRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Retrieves all Bitcoin records ordered by timestamp descending.
        /// </summary>
        /// <returns>
        /// Collection of all BitcoinRecord entities.
        /// </returns>
        public async Task<IEnumerable<BitcoinRecord>> GetAllAsync()
        {

            return await _context.BitcoinRecords
                // Order records newest entries appear first
                .OrderByDescending(r => r.Timestamp)
                // Execute query asynchronously and materialize results
                .ToListAsync();
        }


        /// <summary>
        /// Retrieves a single Bitcoin record by its identifier.
        /// </summary>
        /// <param name="id">Primary key of the BitcoinRecord.</param>
        /// <returns>
        /// BitcoinRecord entity if found; otherwise null.
        /// </returns>
        public async Task<BitcoinRecord> GetByIdAsync(int id)
        {
            return await _context.BitcoinRecords
                // Find first record matching given ID
                .FirstOrDefaultAsync(r => r.Id == id);
        }


        /// <summary>
        /// Retrieves a paginated subset of Bitcoin records.
        /// </summary>
        /// <param name="pageNumber">Current page number (1-based).</param>
        /// <param name="pageSize">Number of records per page.</param>
        /// <returns>
        /// Collection of BitcoinRecord entities for the requested page.
        /// </returns>
        public async Task<IEnumerable<BitcoinRecord>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.BitcoinRecords
                // Order records by timestamp descending
                .OrderByDescending(r => r.Timestamp)
                // Skip records from previous pages
                .Skip((pageNumber - 1) * pageSize)
                // Take only records for current page
                .Take(pageSize)
                // Execute query asynchronously
                .ToListAsync();
        }


        /// <summary>
        /// Returns the total number of Bitcoin records stored in the database.
        /// </summary>
        /// <returns>Total record count.</returns>
        public async Task<int> GetCountAsync()
        {
            // Executes COUNT(*) on BitcoinRecords table
            return await _context.BitcoinRecords.CountAsync();
        }


        /// <summary>
        /// Adds a new Bitcoin record to database.
        /// </summary>
        /// <param name="record">BitcoinRecord entity to add.</param>
        /// <returns>The persisted BitcoinRecord entity.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the record is null.
        /// </exception>
        public async Task<BitcoinRecord> AddAsync(BitcoinRecord record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            // Set timestamp if not provided / was not set explicitly, assign current time
            if (record.Timestamp == default)
                record.Timestamp = DateTime.Now;

            // Add entity to change tracker
            await _context.BitcoinRecords.AddAsync(record);

            // Keep changes in database
            await _context.SaveChangesAsync();

            return record;
        }


        /// <summary>
        /// Updates an existing Bitcoin record.
        /// </summary>
        /// <param name="record">BitcoinRecord entity with updated values.</param>
        /// <returns>The updated BitcoinRecord entity.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the record is null.
        /// </exception>
        public async Task<BitcoinRecord> UpdateAsync(BitcoinRecord record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            // Mark entity as modified
            _context.BitcoinRecords.Update(record);

            // Keep changes in database
            await _context.SaveChangesAsync();

            return record;
        }


        /// <summary>
        /// Updates only the Note field of an existing Bitcoin record.
        /// 
        /// This method is optimized for scenarios where only the user note
        /// is editable (e.g. Saved Data page).
        /// </summary>
        /// <param name="id">Identifier of the record.</param>
        /// <param name="note">New note value.</param>
        /// <returns>
        /// True if the record was found and updated; otherwise false.
        /// </returns>
        public async Task<bool> UpdateNoteAsync(int id, string note)
        {
            // Retrieve the record by ID
            var record = await GetByIdAsync(id);

            // If record does not exist, exit early
            if (record == null)
                return false;

            // Update only the Note field
            record.Note = note ?? string.Empty;

            // Keep changes in database
            await _context.SaveChangesAsync();

            return true;
        }


        /// <summary>
        /// Deletes a Bitcoin record by its identifier.
        /// </summary>
        /// <param name="id">Identifier of the record to delete.</param>
        /// <returns>
        /// True if the record was found and deleted; otherwise false.
        /// </returns>
        public async Task<bool> DeleteAsync(int id)
        {
            // Attempt to retrieve the record
            var record = await GetByIdAsync(id);

            // If record does not exist, nothing to delete
            if (record == null)
                return false;

            // Remove entity from the DbSet
            _context.BitcoinRecords.Remove(record);

            // Persist deletion
            await _context.SaveChangesAsync();

            return true;
        }


        /// <summary>
        /// Deletes multiple Bitcoin records based on provided identifiers.
        /// </summary>
        /// <param name="ids">Collection of record IDs to delete.</param>
        /// <returns>
        /// Number of records successfully deleted.
        /// </returns>
        public async Task<int> DeleteRangeAsync(IEnumerable<int> ids)
        {
            // Validate input collection
            if (ids == null || !ids.Any())
                return 0;

            // Retrieve all matching records from database
            var records = await _context.BitcoinRecords
                .Where(r => ids.Contains(r.Id))
                .ToListAsync();

            // If no records were found, exit early
            if (!records.Any())
                return 0;

            // Remove all retrieved records
            _context.BitcoinRecords.RemoveRange(records);

            // Persist deletion
            await _context.SaveChangesAsync();

            return records.Count;
        }

        /// <summary>
        /// Checks whether a Bitcoin record with the specified ID exists.
        /// </summary>
        /// <param name="id">Identifier to check.</param>
        /// <returns>
        /// True if the record exists; otherwise false.
        /// </returns>
        public async Task<bool> ExistsAsync(int id)
        {
            // Executes an efficient EXISTS query
            return await _context.BitcoinRecords.AnyAsync(r => r.Id == id);
        }
    }
}
