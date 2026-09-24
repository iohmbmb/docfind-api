using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthcareAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUnavailabilityRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UnavailabilityPeriod_DoctorId",
                table: "UnavailabilityPeriod");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                table: "UnavailabilityPeriod",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                table: "UnavailabilityPeriod",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnavailabilityPeriod_DoctorId",
                table: "UnavailabilityPeriod",
                column: "DoctorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UnavailabilityPeriod_DoctorId",
                table: "UnavailabilityPeriod");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                table: "UnavailabilityPeriod",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                table: "UnavailabilityPeriod",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_UnavailabilityPeriod_DoctorId",
                table: "UnavailabilityPeriod",
                column: "DoctorId",
                unique: true);
        }
    }
}
