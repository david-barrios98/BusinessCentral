using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessCentral.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyBusinessNatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyBusinessNature",
                schema: "config",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    BusinessNatureId = table.Column<int>(type: "int", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyBusinessNature", x => new { x.CompanyId, x.BusinessNatureId });
                    table.ForeignKey(
                        name: "FK_CompanyBusinessNature_BusinessNature_BusinessNatureId",
                        column: x => x.BusinessNatureId,
                        principalSchema: "config",
                        principalTable: "BusinessNature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyBusinessNature_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyBusinessNature_BusinessNatureId",
                schema: "config",
                table: "CompanyBusinessNature",
                column: "BusinessNatureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyBusinessNature",
                schema: "config");
        }
    }
}
