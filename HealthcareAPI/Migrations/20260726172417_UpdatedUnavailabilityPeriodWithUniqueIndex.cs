using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthcareAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUnavailabilityPeriodWithUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnavailabilityPeriod_Users_DoctorId",
                table: "UnavailabilityPeriod");

            migrationBuilder.DropIndex(
                name: "IX_UnavailabilityPeriod_DoctorId",
                table: "UnavailabilityPeriod");

            migrationBuilder.CreateIndex(
                name: "IX_UnavailabilityPeriod_DoctorId",
                table: "UnavailabilityPeriod",
                column: "DoctorId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UnavailabilityPeriod_DoctorId",
                table: "UnavailabilityPeriod");

            migrationBuilder.CreateIndex(
                name: "IX_UnavailabilityPeriod_DoctorId",
                table: "UnavailabilityPeriod",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnavailabilityPeriod_Users_DoctorId",
                table: "UnavailabilityPeriod",
                column: "DoctorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
