using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessCentral.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCashOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClosingAmount",
                schema: "business",
                table: "CashSession",
                newName: "ExpectedClosingAmount");

            migrationBuilder.AddColumn<int>(
                name: "ClosedByUserId",
                schema: "business",
                table: "CashSession",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CountedClosingAmount",
                schema: "business",
                table: "CashSession",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DifferenceAmount",
                schema: "business",
                table: "CashSession",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CashMovement",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CashSessionId = table.Column<long>(type: "bigint", nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ReasonCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReferenceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PerformedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashMovement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashMovement_CashSession_CashSessionId",
                        column: x => x.CashSessionId,
                        principalSchema: "business",
                        principalTable: "CashSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashMovement_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashMovement_UsersInfo_PerformedByUserId",
                        column: x => x.PerformedByUserId,
                        principalSchema: "auth",
                        principalTable: "UsersInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashSession_ClosedByUserId",
                schema: "business",
                table: "CashSession",
                column: "ClosedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CashMovement_CashSessionId",
                schema: "business",
                table: "CashMovement",
                column: "CashSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_CashMovement_CompanyId",
                schema: "business",
                table: "CashMovement",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CashMovement_PerformedByUserId",
                schema: "business",
                table: "CashMovement",
                column: "PerformedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CashSession_UsersInfo_ClosedByUserId",
                schema: "business",
                table: "CashSession",
                column: "ClosedByUserId",
                principalSchema: "auth",
                principalTable: "UsersInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashSession_UsersInfo_ClosedByUserId",
                schema: "business",
                table: "CashSession");

            migrationBuilder.DropTable(
                name: "CashMovement",
                schema: "business");

            migrationBuilder.DropIndex(
                name: "IX_CashSession_ClosedByUserId",
                schema: "business",
                table: "CashSession");

            migrationBuilder.DropColumn(
                name: "ClosedByUserId",
                schema: "business",
                table: "CashSession");

            migrationBuilder.DropColumn(
                name: "CountedClosingAmount",
                schema: "business",
                table: "CashSession");

            migrationBuilder.DropColumn(
                name: "DifferenceAmount",
                schema: "business",
                table: "CashSession");

            migrationBuilder.RenameColumn(
                name: "ExpectedClosingAmount",
                schema: "business",
                table: "CashSession",
                newName: "ClosingAmount");
        }
    }
}
