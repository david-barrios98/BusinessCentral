using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessCentral.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommerceVariantsAndPurchasing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "VariantId",
                schema: "business",
                table: "InventoryMovement",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductAttribute",
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
                    table.PrimaryKey("PK_ProductAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAttribute_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariant",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VariantName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PriceOverride = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CostOverride = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariant_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductVariant_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "business",
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Supplier_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributeOption",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    AttributeId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAttributeOption_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductAttributeOption_ProductAttribute_AttributeId",
                        column: x => x.AttributeId,
                        principalSchema: "business",
                        principalTable: "ProductAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseReceipt",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupplierInvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DefaultToLocationId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseReceipt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseReceipt_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseReceipt_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "business",
                        principalTable: "Supplier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariantOption",
                schema: "business",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    VariantId = table.Column<long>(type: "bigint", nullable: false),
                    AttributeId = table.Column<int>(type: "int", nullable: false),
                    OptionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariantOption", x => new { x.CompanyId, x.VariantId, x.AttributeId, x.OptionId });
                    table.ForeignKey(
                        name: "FK_ProductVariantOption_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductVariantOption_ProductAttributeOption_OptionId",
                        column: x => x.OptionId,
                        principalSchema: "business",
                        principalTable: "ProductAttributeOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductVariantOption_ProductAttribute_AttributeId",
                        column: x => x.AttributeId,
                        principalSchema: "business",
                        principalTable: "ProductAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductVariantOption_ProductVariant_VariantId",
                        column: x => x.VariantId,
                        principalSchema: "business",
                        principalTable: "ProductVariant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseReceiptLine",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ReceiptId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    VariantId = table.Column<long>(type: "bigint", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ToLocationId = table.Column<long>(type: "bigint", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseReceiptLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseReceiptLine_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseReceiptLine_ProductVariant_VariantId",
                        column: x => x.VariantId,
                        principalSchema: "business",
                        principalTable: "ProductVariant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseReceiptLine_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "business",
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseReceiptLine_PurchaseReceipt_ReceiptId",
                        column: x => x.ReceiptId,
                        principalSchema: "business",
                        principalTable: "PurchaseReceipt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovement_VariantId",
                schema: "business",
                table: "InventoryMovement",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttribute_CompanyId",
                schema: "business",
                table: "ProductAttribute",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeOption_AttributeId",
                schema: "business",
                table: "ProductAttributeOption",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeOption_CompanyId",
                schema: "business",
                table: "ProductAttributeOption",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariant_CompanyId",
                schema: "business",
                table: "ProductVariant",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariant_ProductId",
                schema: "business",
                table: "ProductVariant",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantOption_AttributeId",
                schema: "business",
                table: "ProductVariantOption",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantOption_OptionId",
                schema: "business",
                table: "ProductVariantOption",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantOption_VariantId",
                schema: "business",
                table: "ProductVariantOption",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceipt_CompanyId",
                schema: "business",
                table: "PurchaseReceipt",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceipt_SupplierId",
                schema: "business",
                table: "PurchaseReceipt",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceiptLine_CompanyId",
                schema: "business",
                table: "PurchaseReceiptLine",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceiptLine_ProductId",
                schema: "business",
                table: "PurchaseReceiptLine",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceiptLine_ReceiptId",
                schema: "business",
                table: "PurchaseReceiptLine",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceiptLine_VariantId",
                schema: "business",
                table: "PurchaseReceiptLine",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_CompanyId",
                schema: "business",
                table: "Supplier",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryMovement_ProductVariant_VariantId",
                schema: "business",
                table: "InventoryMovement",
                column: "VariantId",
                principalSchema: "business",
                principalTable: "ProductVariant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovement_ProductVariant_VariantId",
                schema: "business",
                table: "InventoryMovement");

            migrationBuilder.DropTable(
                name: "ProductVariantOption",
                schema: "business");

            migrationBuilder.DropTable(
                name: "PurchaseReceiptLine",
                schema: "business");

            migrationBuilder.DropTable(
                name: "ProductAttributeOption",
                schema: "business");

            migrationBuilder.DropTable(
                name: "ProductVariant",
                schema: "business");

            migrationBuilder.DropTable(
                name: "PurchaseReceipt",
                schema: "business");

            migrationBuilder.DropTable(
                name: "ProductAttribute",
                schema: "business");

            migrationBuilder.DropTable(
                name: "Supplier",
                schema: "business");

            migrationBuilder.DropIndex(
                name: "IX_InventoryMovement_VariantId",
                schema: "business",
                table: "InventoryMovement");

            migrationBuilder.DropColumn(
                name: "VariantId",
                schema: "business",
                table: "InventoryMovement");
        }
    }
}
