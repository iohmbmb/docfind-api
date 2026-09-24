using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthcareAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedMockFlagForMedicalDataSeeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMock",
                table: "Users",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMock",
                table: "UnavailabilityPeriod",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMock",
                table: "DoctorWorkingHours",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMock",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsMock",
                table: "UnavailabilityPeriod");

            migrationBuilder.DropColumn(
                name: "IsMock",
                table: "DoctorWorkingHours");
        }
    }
}
