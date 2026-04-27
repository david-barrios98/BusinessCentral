using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessCentral.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProfessionalizeBusinessCatalogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CustomerId",
                schema: "business",
                table: "ServiceOrder",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "VehicleId",
                schema: "business",
                table: "ServiceOrder",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                schema: "business",
                table: "Product",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                schema: "business",
                table: "Product",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "business",
                table: "Product",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxCode",
                schema: "business",
                table: "Product",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TrackInventory",
                schema: "business",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ProductFormTypeId",
                schema: "business",
                table: "HarvestLot",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcessTypeId",
                schema: "business",
                table: "CoffeeProcessStep",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Category",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Category_Category_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalSchema: "business",
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Category_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CoffeeProcessType",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoffeeProcessType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoffeeProcessType_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CoffeeProductFormType",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoffeeProductFormType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoffeeProductFormType_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customer_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategory",
                schema: "business",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategory", x => new { x.CompanyId, x.ProductId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_ProductCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "business",
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductCategory_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductCategory_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "business",
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vehicle",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Plate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicle_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicle_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "business",
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrder_CustomerId",
                schema: "business",
                table: "ServiceOrder",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrder_VehicleId",
                schema: "business",
                table: "ServiceOrder",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_HarvestLot_ProductFormTypeId",
                schema: "business",
                table: "HarvestLot",
                column: "ProductFormTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CoffeeProcessStep_ProcessTypeId",
                schema: "business",
                table: "CoffeeProcessStep",
                column: "ProcessTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_CompanyId",
                schema: "business",
                table: "Category",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_ParentCategoryId",
                schema: "business",
                table: "Category",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CoffeeProcessType_CompanyId",
                schema: "business",
                table: "CoffeeProcessType",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CoffeeProductFormType_CompanyId",
                schema: "business",
                table: "CoffeeProductFormType",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CompanyId",
                schema: "business",
                table: "Customer",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategory_CategoryId",
                schema: "business",
                table: "ProductCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategory_ProductId",
                schema: "business",
                table: "ProductCategory",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_CompanyId",
                schema: "business",
                table: "Vehicle",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_CustomerId",
                schema: "business",
                table: "Vehicle",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CoffeeProcessStep_CoffeeProcessType_ProcessTypeId",
                schema: "business",
                table: "CoffeeProcessStep",
                column: "ProcessTypeId",
                principalSchema: "business",
                principalTable: "CoffeeProcessType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HarvestLot_CoffeeProductFormType_ProductFormTypeId",
                schema: "business",
                table: "HarvestLot",
                column: "ProductFormTypeId",
                principalSchema: "business",
                principalTable: "CoffeeProductFormType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrder_Customer_CustomerId",
                schema: "business",
                table: "ServiceOrder",
                column: "CustomerId",
                principalSchema: "business",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrder_Vehicle_VehicleId",
                schema: "business",
                table: "ServiceOrder",
                column: "VehicleId",
                principalSchema: "business",
                principalTable: "Vehicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoffeeProcessStep_CoffeeProcessType_ProcessTypeId",
                schema: "business",
                table: "CoffeeProcessStep");

            migrationBuilder.DropForeignKey(
                name: "FK_HarvestLot_CoffeeProductFormType_ProductFormTypeId",
                schema: "business",
                table: "HarvestLot");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrder_Customer_CustomerId",
                schema: "business",
                table: "ServiceOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrder_Vehicle_VehicleId",
                schema: "business",
                table: "ServiceOrder");

            migrationBuilder.DropTable(
                name: "CoffeeProcessType",
                schema: "business");

            migrationBuilder.DropTable(
                name: "CoffeeProductFormType",
                schema: "business");

            migrationBuilder.DropTable(
                name: "ProductCategory",
                schema: "business");

            migrationBuilder.DropTable(
                name: "Vehicle",
                schema: "business");

            migrationBuilder.DropTable(
                name: "Category",
                schema: "business");

            migrationBuilder.DropTable(
                name: "Customer",
                schema: "business");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOrder_CustomerId",
                schema: "business",
                table: "ServiceOrder");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOrder_VehicleId",
                schema: "business",
                table: "ServiceOrder");

            migrationBuilder.DropIndex(
                name: "IX_HarvestLot_ProductFormTypeId",
                schema: "business",
                table: "HarvestLot");

            migrationBuilder.DropIndex(
                name: "IX_CoffeeProcessStep_ProcessTypeId",
                schema: "business",
                table: "CoffeeProcessStep");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                schema: "business",
                table: "ServiceOrder");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                schema: "business",
                table: "ServiceOrder");

            migrationBuilder.DropColumn(
                name: "Barcode",
                schema: "business",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Cost",
                schema: "business",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "business",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "TaxCode",
                schema: "business",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "TrackInventory",
                schema: "business",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ProductFormTypeId",
                schema: "business",
                table: "HarvestLot");

            migrationBuilder.DropColumn(
                name: "ProcessTypeId",
                schema: "business",
                table: "CoffeeProcessStep");
        }
    }
}
