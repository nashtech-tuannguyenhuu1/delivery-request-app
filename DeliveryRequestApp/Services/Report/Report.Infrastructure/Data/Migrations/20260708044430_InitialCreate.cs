using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Report.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyTrackings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    NewCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    AssignedCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    DeliveredCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ReturnedCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTrackings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyTrackings_Date",
                table: "DailyTrackings",
                column: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyTrackings");
        }
    }
}
