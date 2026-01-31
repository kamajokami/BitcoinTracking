using Microsoft.EntityFrameworkCore;
using BitcoinTracking.DAL.Entities;


namespace BitcoinTracking.DAL.DataContext
{
    /// <summary>
    /// Database context for Bitcoin Tracking application
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// DbSet for Bitcoin records
        /// </summary>
        public DbSet<BitcoinRecord> BitcoinRecords { get; set; }

        /// <summary>
        /// Model configuration using Fluent API
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all Fluent API configurations from this assembly - separate files
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
