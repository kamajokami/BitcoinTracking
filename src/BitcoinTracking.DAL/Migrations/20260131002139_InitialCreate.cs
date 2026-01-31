using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitcoinTracking.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BitcoinRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    PriceBtcEur = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExchangeRateEurCzk = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PriceBtcCzk = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BitcoinRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BitcoinRecords_Timestamp",
                table: "BitcoinRecords",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BitcoinRecords");
        }
    }
}
