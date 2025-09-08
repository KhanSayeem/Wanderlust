using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourismApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTouristProfileRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TouristProfileId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TouristProfileId",
                table: "Bookings",
                column: "TouristProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TouristProfiles_TouristProfileId",
                table: "Bookings",
                column: "TouristProfileId",
                principalTable: "TouristProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TouristProfiles_TouristProfileId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TouristProfileId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TouristProfileId",
                table: "Bookings");
        }
    }
}
