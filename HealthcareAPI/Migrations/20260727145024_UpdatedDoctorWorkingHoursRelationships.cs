using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthcareAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDoctorWorkingHoursRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DoctorWorkingHours_DoctorId",
                table: "DoctorWorkingHours");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorWorkingHours_DoctorId",
                table: "DoctorWorkingHours",
                column: "DoctorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DoctorWorkingHours_DoctorId",
                table: "DoctorWorkingHours");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorWorkingHours_DoctorId",
                table: "DoctorWorkingHours",
                column: "DoctorId",
                unique: true);
        }
    }
}
