using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessCentral.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFulfillmentMethods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FulfillmentDetails",
                schema: "business",
                table: "ServiceOrder",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FulfillmentMethodCode",
                schema: "business",
                table: "ServiceOrder",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FulfillmentDetails",
                schema: "business",
                table: "PosTicket",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FulfillmentMethodCode",
                schema: "business",
                table: "PosTicket",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FinancialBootstrapStatus",
                schema: "business",
                table: "Companies",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "NOT_STARTED");

            migrationBuilder.CreateTable(
                name: "FulfillmentMethod",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AppliesTo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FulfillmentMethod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessNatureFulfillmentMethod",
                schema: "config",
                columns: table => new
                {
                    BusinessNatureId = table.Column<int>(type: "int", nullable: false),
                    FulfillmentMethodId = table.Column<int>(type: "int", nullable: false),
                    IsDefaultEnabled = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessNatureFulfillmentMethod", x => new { x.BusinessNatureId, x.FulfillmentMethodId });
                    table.ForeignKey(
                        name: "FK_BusinessNatureFulfillmentMethod_BusinessNature_BusinessNatureId",
                        column: x => x.BusinessNatureId,
                        principalSchema: "config",
                        principalTable: "BusinessNature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusinessNatureFulfillmentMethod_FulfillmentMethod_FulfillmentMethodId",
                        column: x => x.FulfillmentMethodId,
                        principalSchema: "config",
                        principalTable: "FulfillmentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyFulfillmentMethod",
                schema: "config",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    FulfillmentMethodId = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyFulfillmentMethod", x => new { x.CompanyId, x.FulfillmentMethodId });
                    table.ForeignKey(
                        name: "FK_CompanyFulfillmentMethod_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyFulfillmentMethod_FulfillmentMethod_FulfillmentMethodId",
                        column: x => x.FulfillmentMethodId,
                        principalSchema: "config",
                        principalTable: "FulfillmentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessNatureFulfillmentMethod_FulfillmentMethodId",
                schema: "config",
                table: "BusinessNatureFulfillmentMethod",
                column: "FulfillmentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyFulfillmentMethod_FulfillmentMethodId",
                schema: "config",
                table: "CompanyFulfillmentMethod",
                column: "FulfillmentMethodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessNatureFulfillmentMethod",
                schema: "config");

            migrationBuilder.DropTable(
                name: "CompanyFulfillmentMethod",
                schema: "config");

            migrationBuilder.DropTable(
                name: "FulfillmentMethod",
                schema: "config");

            migrationBuilder.DropColumn(
                name: "FulfillmentDetails",
                schema: "business",
                table: "ServiceOrder");

            migrationBuilder.DropColumn(
                name: "FulfillmentMethodCode",
                schema: "business",
                table: "ServiceOrder");

            migrationBuilder.DropColumn(
                name: "FulfillmentDetails",
                schema: "business",
                table: "PosTicket");

            migrationBuilder.DropColumn(
                name: "FulfillmentMethodCode",
                schema: "business",
                table: "PosTicket");

            migrationBuilder.AlterColumn<string>(
                name: "FinancialBootstrapStatus",
                schema: "business",
                table: "Companies",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "NOT_STARTED",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }
    }
}
