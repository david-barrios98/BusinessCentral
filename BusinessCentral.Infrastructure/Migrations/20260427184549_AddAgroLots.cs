using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessCentral.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAgroLots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgroFeedLog",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    LotId = table.Column<long>(type: "bigint", nullable: false),
                    FeedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FeedProductId = table.Column<int>(type: "int", nullable: false),
                    FeedVariantId = table.Column<long>(type: "bigint", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FromLocationId = table.Column<long>(type: "bigint", nullable: true),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgroFeedLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgroFeedLog_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgroHarvest",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    LotId = table.Column<long>(type: "bigint", nullable: false),
                    HarvestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OutputProductId = table.Column<int>(type: "int", nullable: false),
                    OutputVariantId = table.Column<long>(type: "bigint", nullable: true),
                    Units = table.Column<int>(type: "int", nullable: false),
                    TotalWeightKg = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ToLocationId = table.Column<long>(type: "bigint", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgroHarvest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgroHarvest_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgroLot",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InitialUnits = table.Column<int>(type: "int", nullable: false),
                    CurrentUnits = table.Column<int>(type: "int", nullable: false),
                    InitialAvgWeightKg = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgroLot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgroLot_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgroMortalityLog",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    LotId = table.Column<long>(type: "bigint", nullable: false),
                    MortalityDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Units = table.Column<int>(type: "int", nullable: false),
                    AvgWeightKg = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgroMortalityLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgroMortalityLog_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgroFeedLog_CompanyId",
                schema: "business",
                table: "AgroFeedLog",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AgroHarvest_CompanyId",
                schema: "business",
                table: "AgroHarvest",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AgroLot_CompanyId",
                schema: "business",
                table: "AgroLot",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AgroMortalityLog_CompanyId",
                schema: "business",
                table: "AgroMortalityLog",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgroFeedLog",
                schema: "business");

            migrationBuilder.DropTable(
                name: "AgroHarvest",
                schema: "business");

            migrationBuilder.DropTable(
                name: "AgroLot",
                schema: "business");

            migrationBuilder.DropTable(
                name: "AgroMortalityLog",
                schema: "business");
        }
    }
}
