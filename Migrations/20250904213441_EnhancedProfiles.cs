using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourismApp.Migrations
{
    /// <inheritdoc />
    public partial class EnhancedProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TouristProfiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "TouristProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact",
                table: "TouristProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "TouristProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImagePath",
                table: "TouristProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AgencyProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "AgencyProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AgencyProfiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ProfileImagePath",
                table: "AgencyProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServicesOffered",
                table: "AgencyProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TourGuideInfo",
                table: "AgencyProfiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TouristProfiles");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "TouristProfiles");

            migrationBuilder.DropColumn(
                name: "EmergencyContact",
                table: "TouristProfiles");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "TouristProfiles");

            migrationBuilder.DropColumn(
                name: "ProfileImagePath",
                table: "TouristProfiles");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AgencyProfiles");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "AgencyProfiles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AgencyProfiles");

            migrationBuilder.DropColumn(
                name: "ProfileImagePath",
                table: "AgencyProfiles");

            migrationBuilder.DropColumn(
                name: "ServicesOffered",
                table: "AgencyProfiles");

            migrationBuilder.DropColumn(
                name: "TourGuideInfo",
                table: "AgencyProfiles");
        }
    }
}
