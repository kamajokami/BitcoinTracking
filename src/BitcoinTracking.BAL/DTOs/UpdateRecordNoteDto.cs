

namespace BitcoinTracking.BAL.DTOs
{
    /// <summary>
    /// DTO for updating the note field of a saved record
    /// Page Saved Data
    /// </summary>
    public class UpdateRecordNoteDto
    {
        /// <summary>
        /// Record ID to update
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// New note value (REQUIRED by assignment)
        /// </summary>
        public string Note { get; set; }
    }
}
