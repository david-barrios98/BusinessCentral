using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessCentral.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStorageLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FromLocationId",
                schema: "business",
                table: "InventoryMovement",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ToLocationId",
                schema: "business",
                table: "InventoryMovement",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StorageLocation",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    FacilityId = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ParentLocationId = table.Column<long>(type: "bigint", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorageLocation_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StorageLocation_Facility_FacilityId",
                        column: x => x.FacilityId,
                        principalSchema: "business",
                        principalTable: "Facility",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StorageLocation_StorageLocation_ParentLocationId",
                        column: x => x.ParentLocationId,
                        principalSchema: "business",
                        principalTable: "StorageLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovement_FromLocationId",
                schema: "business",
                table: "InventoryMovement",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovement_ToLocationId",
                schema: "business",
                table: "InventoryMovement",
                column: "ToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocation_CompanyId",
                schema: "business",
                table: "StorageLocation",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocation_FacilityId",
                schema: "business",
                table: "StorageLocation",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocation_ParentLocationId",
                schema: "business",
                table: "StorageLocation",
                column: "ParentLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryMovement_StorageLocation_FromLocationId",
                schema: "business",
                table: "InventoryMovement",
                column: "FromLocationId",
                principalSchema: "business",
                principalTable: "StorageLocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryMovement_StorageLocation_ToLocationId",
                schema: "business",
                table: "InventoryMovement",
                column: "ToLocationId",
                principalSchema: "business",
                principalTable: "StorageLocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovement_StorageLocation_FromLocationId",
                schema: "business",
                table: "InventoryMovement");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovement_StorageLocation_ToLocationId",
                schema: "business",
                table: "InventoryMovement");

            migrationBuilder.DropTable(
                name: "StorageLocation",
                schema: "business");

            migrationBuilder.DropIndex(
                name: "IX_InventoryMovement_FromLocationId",
                schema: "business",
                table: "InventoryMovement");

            migrationBuilder.DropIndex(
                name: "IX_InventoryMovement_ToLocationId",
                schema: "business",
                table: "InventoryMovement");

            migrationBuilder.DropColumn(
                name: "FromLocationId",
                schema: "business",
                table: "InventoryMovement");

            migrationBuilder.DropColumn(
                name: "ToLocationId",
                schema: "business",
                table: "InventoryMovement");
        }
    }
}
