using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessCentral.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessNatureTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BusinessNatureId",
                schema: "business",
                table: "Companies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BusinessNature",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessNature", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessNatureModule",
                schema: "config",
                columns: table => new
                {
                    BusinessNatureId = table.Column<int>(type: "int", nullable: false),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    IsDefaultEnabled = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessNatureModule", x => new { x.BusinessNatureId, x.ModuleId });
                    table.ForeignKey(
                        name: "FK_BusinessNatureModule_BusinessNature_BusinessNatureId",
                        column: x => x.BusinessNatureId,
                        principalSchema: "config",
                        principalTable: "BusinessNature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusinessNatureModule_Module_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "config",
                        principalTable: "Module",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_BusinessNatureId",
                schema: "business",
                table: "Companies",
                column: "BusinessNatureId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessNatureModule_ModuleId",
                schema: "config",
                table: "BusinessNatureModule",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_BusinessNature_BusinessNatureId",
                schema: "business",
                table: "Companies",
                column: "BusinessNatureId",
                principalSchema: "config",
                principalTable: "BusinessNature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_BusinessNature_BusinessNatureId",
                schema: "business",
                table: "Companies");

            migrationBuilder.DropTable(
                name: "BusinessNatureModule",
                schema: "config");

            migrationBuilder.DropTable(
                name: "BusinessNature",
                schema: "config");

            migrationBuilder.DropIndex(
                name: "IX_Companies_BusinessNatureId",
                schema: "business",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "BusinessNatureId",
                schema: "business",
                table: "Companies");
        }
    }
}
