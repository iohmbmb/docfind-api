using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthcareAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDoctorModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaitlistAppointments_Appointments_AppointmentId",
                table: "WaitlistAppointments");

            migrationBuilder.AlterColumn<Guid>(
                name: "AppointmentId",
                table: "WaitlistAppointments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "PracticePostcode",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PracticeState",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PracticeSuburb",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WaitlistAppointments_Appointments_AppointmentId",
                table: "WaitlistAppointments",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaitlistAppointments_Appointments_AppointmentId",
                table: "WaitlistAppointments");

            migrationBuilder.DropColumn(
                name: "PracticePostcode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PracticeState",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PracticeSuburb",
                table: "Users");

            migrationBuilder.AlterColumn<Guid>(
                name: "AppointmentId",
                table: "WaitlistAppointments",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WaitlistAppointments_Appointments_AppointmentId",
                table: "WaitlistAppointments",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
