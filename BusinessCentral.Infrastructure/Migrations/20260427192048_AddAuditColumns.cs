using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessCentral.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "business",
                table: "Supplier",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                schema: "business",
                table: "Supplier",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "business",
                table: "StorageLocation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                schema: "business",
                table: "StorageLocation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "business",
                table: "ProductVariant",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                schema: "business",
                table: "ProductVariant",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "business",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                schema: "business",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "business",
                table: "AgroLot",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                schema: "business",
                table: "AgroLot",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "business",
                table: "Supplier");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                schema: "business",
                table: "Supplier");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "business",
                table: "StorageLocation");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                schema: "business",
                table: "StorageLocation");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "business",
                table: "ProductVariant");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                schema: "business",
                table: "ProductVariant");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "business",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                schema: "business",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "business",
                table: "AgroLot");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                schema: "business",
                table: "AgroLot");
        }
    }
}
