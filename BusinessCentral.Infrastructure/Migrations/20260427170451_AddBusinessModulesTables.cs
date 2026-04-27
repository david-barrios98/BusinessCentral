using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessCentral.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessModulesTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CashSession",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    OpenedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpenedByUserId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OpeningAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClosingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashSession_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashSession_UsersInfo_OpenedByUserId",
                        column: x => x.OpenedByUserId,
                        principalSchema: "auth",
                        principalTable: "UsersInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyModule",
                schema: "config",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyModule", x => new { x.CompanyId, x.ModuleId });
                    table.ForeignKey(
                        name: "FK_CompanyModule_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyModule_Module_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "config",
                        principalTable: "Module",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Deduction",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deduction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deduction_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deduction_UsersInfo_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "UsersInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProfile",
                schema: "business",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    IsEmployee = table.Column<bool>(type: "bit", nullable: false),
                    ActiveEmployee = table.Column<bool>(type: "bit", nullable: false),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LodgingIncluded = table.Column<bool>(type: "bit", nullable: false),
                    LodgingLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MattressIncluded = table.Column<bool>(type: "bit", nullable: false),
                    MealPlanCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MealUnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProfile", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_EmployeeProfile_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeProfile_UsersInfo_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "UsersInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FarmZone",
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
                    table.PrimaryKey("PK_FarmZone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FarmZone_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoanAdvance",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanAdvance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanAdvance_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanAdvance_UsersInfo_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "UsersInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayScheme",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayScheme", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayScheme_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCatalog",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCatalog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceCatalog_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceOrder",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VehicleType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Plate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceOrder_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PosTicket",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CashSessionId = table.Column<long>(type: "bigint", nullable: true),
                    TicketDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosTicket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosTicket_CashSession_CashSessionId",
                        column: x => x.CashSessionId,
                        principalSchema: "business",
                        principalTable: "CashSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosTicket_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HarvestLot",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ZoneId = table.Column<int>(type: "int", nullable: true),
                    HarvestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductForm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QuantityKg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HarvestLot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HarvestLot_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HarvestLot_FarmZone_ZoneId",
                        column: x => x.ZoneId,
                        principalSchema: "business",
                        principalTable: "FarmZone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkLog",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    WorkDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaySchemeId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkLog_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkLog_PayScheme_PaySchemeId",
                        column: x => x.PaySchemeId,
                        principalSchema: "business",
                        principalTable: "PayScheme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkLog_UsersInfo_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "UsersInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryMovement",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    MoveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryMovement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryMovement_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryMovement_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "business",
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceOrderLine",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmployeeUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOrderLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceOrderLine_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceOrderLine_ServiceCatalog_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "business",
                        principalTable: "ServiceCatalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceOrderLine_ServiceOrder_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "business",
                        principalTable: "ServiceOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceOrderLine_UsersInfo_EmployeeUserId",
                        column: x => x.EmployeeUserId,
                        principalSchema: "auth",
                        principalTable: "UsersInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PosPayment",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    TicketId = table.Column<long>(type: "bigint", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosPayment_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosPayment_PosTicket_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "business",
                        principalTable: "PosTicket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PosTicketLine",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    TicketId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosTicketLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosTicketLine_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosTicketLine_PosTicket_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "business",
                        principalTable: "PosTicket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosTicketLine_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "business",
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CoffeeProcessStep",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    HarvestLotId = table.Column<long>(type: "bigint", nullable: false),
                    StepDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StepType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InputKg = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OutputKg = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoffeeProcessStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoffeeProcessStep_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CoffeeProcessStep_HarvestLot_HarvestLotId",
                        column: x => x.HarvestLotId,
                        principalSchema: "business",
                        principalTable: "HarvestLot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashSession_CompanyId",
                schema: "business",
                table: "CashSession",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CashSession_OpenedByUserId",
                schema: "business",
                table: "CashSession",
                column: "OpenedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CoffeeProcessStep_CompanyId",
                schema: "business",
                table: "CoffeeProcessStep",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CoffeeProcessStep_HarvestLotId",
                schema: "business",
                table: "CoffeeProcessStep",
                column: "HarvestLotId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyModule_ModuleId",
                schema: "config",
                table: "CompanyModule",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Deduction_CompanyId",
                schema: "business",
                table: "Deduction",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Deduction_UserId",
                schema: "business",
                table: "Deduction",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProfile_CompanyId",
                schema: "business",
                table: "EmployeeProfile",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_FarmZone_CompanyId",
                schema: "business",
                table: "FarmZone",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_HarvestLot_CompanyId",
                schema: "business",
                table: "HarvestLot",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_HarvestLot_ZoneId",
                schema: "business",
                table: "HarvestLot",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovement_CompanyId",
                schema: "business",
                table: "InventoryMovement",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovement_ProductId",
                schema: "business",
                table: "InventoryMovement",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanAdvance_CompanyId",
                schema: "business",
                table: "LoanAdvance",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanAdvance_UserId",
                schema: "business",
                table: "LoanAdvance",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PayScheme_CompanyId",
                schema: "business",
                table: "PayScheme",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PosPayment_CompanyId",
                schema: "business",
                table: "PosPayment",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PosPayment_TicketId",
                schema: "business",
                table: "PosPayment",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTicket_CashSessionId",
                schema: "business",
                table: "PosTicket",
                column: "CashSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTicket_CompanyId",
                schema: "business",
                table: "PosTicket",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTicketLine_CompanyId",
                schema: "business",
                table: "PosTicketLine",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTicketLine_ProductId",
                schema: "business",
                table: "PosTicketLine",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTicketLine_TicketId",
                schema: "business",
                table: "PosTicketLine",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CompanyId",
                schema: "business",
                table: "Product",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCatalog_CompanyId",
                schema: "business",
                table: "ServiceCatalog",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrder_CompanyId",
                schema: "business",
                table: "ServiceOrder",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrderLine_CompanyId",
                schema: "business",
                table: "ServiceOrderLine",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrderLine_EmployeeUserId",
                schema: "business",
                table: "ServiceOrderLine",
                column: "EmployeeUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrderLine_OrderId",
                schema: "business",
                table: "ServiceOrderLine",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrderLine_ServiceId",
                schema: "business",
                table: "ServiceOrderLine",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLog_CompanyId",
                schema: "business",
                table: "WorkLog",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLog_PaySchemeId",
                schema: "business",
                table: "WorkLog",
                column: "PaySchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLog_UserId",
                schema: "business",
                table: "WorkLog",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoffeeProcessStep",
                schema: "business");

            migrationBuilder.DropTable(
                name: "CompanyModule",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Deduction",
                schema: "business");

            migrationBuilder.DropTable(
                name: "EmployeeProfile",
                schema: "business");

            migrationBuilder.DropTable(
                name: "InventoryMovement",
                schema: "business");

            migrationBuilder.DropTable(
                name: "LoanAdvance",
                schema: "business");

            migrationBuilder.DropTable(
                name: "PosPayment",
                schema: "business");

            migrationBuilder.DropTable(
                name: "PosTicketLine",
                schema: "business");

            migrationBuilder.DropTable(
                name: "ServiceOrderLine",
                schema: "business");

            migrationBuilder.DropTable(
                name: "WorkLog",
                schema: "business");

            migrationBuilder.DropTable(
                name: "HarvestLot",
                schema: "business");

            migrationBuilder.DropTable(
                name: "PosTicket",
                schema: "business");

            migrationBuilder.DropTable(
                name: "Product",
                schema: "business");

            migrationBuilder.DropTable(
                name: "ServiceCatalog",
                schema: "business");

            migrationBuilder.DropTable(
                name: "ServiceOrder",
                schema: "business");

            migrationBuilder.DropTable(
                name: "PayScheme",
                schema: "business");

            migrationBuilder.DropTable(
                name: "FarmZone",
                schema: "business");

            migrationBuilder.DropTable(
                name: "CashSession",
                schema: "business");
        }
    }
}
