using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthcareAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDoctorModeWithConsultType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_ConsultationType_ConsultationTypeId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "ConsultationType");

            migrationBuilder.DropIndex(
                name: "IX_Users_ConsultationTypeId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "ConsultationTypeId",
                table: "Users",
                newName: "ConsultationType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConsultationType",
                table: "Users",
                newName: "ConsultationTypeId");

            migrationBuilder.CreateTable(
                name: "ConsultationType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Existing = table.Column<string>(type: "TEXT", nullable: false),
                    New = table.Column<string>(type: "TEXT", nullable: false)
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
    }
}
