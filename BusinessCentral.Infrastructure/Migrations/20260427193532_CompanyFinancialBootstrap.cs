using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessCentral.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CompanyFinancialBootstrap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FinancialBootstrapNotes",
                schema: "business",
                table: "Companies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinancialBootstrapStatus",
                schema: "business",
                table: "Companies",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "NOT_STARTED");

            migrationBuilder.AddColumn<DateTime>(
                name: "FinancialOperatingStartDateUtc",
                schema: "business",
                table: "Companies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinancialStartupMode",
                schema: "business",
                table: "Companies",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinancialBootstrapNotes",
                schema: "business",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "FinancialBootstrapStatus",
                schema: "business",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "FinancialOperatingStartDateUtc",
                schema: "business",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "FinancialStartupMode",
                schema: "business",
                table: "Companies");
        }
    }
}
