using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitcoinTracking.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToBitcoinRecordNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BitcoinRecords_Note",
                table: "BitcoinRecords",
                column: "Note",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BitcoinRecords_Note",
                table: "BitcoinRecords");
        }
    }
}
