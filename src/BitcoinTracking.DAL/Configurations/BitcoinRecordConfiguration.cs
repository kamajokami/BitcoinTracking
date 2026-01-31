using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BitcoinTracking.DAL.Entities;
using BitcoinTracking.Shared.Constants;

namespace BitcoinTracking.DAL.Configurations
{
    /// <summary>
    /// Fluent API configuration for BitcoinRecord entity
    /// </summary>
    public class BitcoinRecordConfiguration : IEntityTypeConfiguration<BitcoinRecord>
    {
        public void Configure(EntityTypeBuilder<BitcoinRecord> builder)
        {
            // Table name
            builder.ToTable("BitcoinRecords");

            // Primary key
            builder.HasKey(b => b.Id);

            // Properties
            builder.Property(b => b.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(b => b.Timestamp)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()"); // SQL Server default

            builder.Property(b => b.PriceBtcEur)
                .IsRequired()
                .HasColumnType("decimal(18,2)"); // Max 18 digits, 2 decimal places

            builder.Property(b => b.ExchangeRateEurCzk)
                .IsRequired()
                .HasColumnType("decimal(18,4)"); // 4 decimals for exchange rate precision

            builder.Property(b => b.PriceBtcCzk)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.Note)
                .IsRequired()
                .HasMaxLength(AppConstants.Validation.MaxNoteLength)
                .HasDefaultValue(""); // Empty string as default

            // Indexes
            builder.HasIndex(b => b.Timestamp)
                .HasDatabaseName("IX_BitcoinRecords_Timestamp");
        }
    }
}
