using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthcareAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedConsultationTypeToDoctorModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConsultationTypeId",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConsultationType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    New = table.Column<string>(type: "TEXT", nullable: false),
                    Existing = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_ConsultationTypeId",
                table: "Users",
                column: "ConsultationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_ConsultationType_ConsultationTypeId",
                table: "Users",
                column: "ConsultationTypeId",
                principalTable: "ConsultationType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_ConsultationType_ConsultationTypeId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "ConsultationType");

            migrationBuilder.DropIndex(
                name: "IX_Users_ConsultationTypeId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ConsultationTypeId",
                table: "Users");
        }
    }
}
