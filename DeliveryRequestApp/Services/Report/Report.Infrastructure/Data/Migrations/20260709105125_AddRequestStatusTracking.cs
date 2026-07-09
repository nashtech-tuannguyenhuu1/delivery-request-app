using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Report.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestStatusTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestStatusTrackings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestStatusTrackings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestStatusTrackings_RequestId",
                table: "RequestStatusTrackings",
                column: "RequestId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestStatusTrackings");
        }
    }
}
