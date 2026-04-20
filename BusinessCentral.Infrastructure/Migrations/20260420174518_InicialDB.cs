using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessCentral.Infrastructure.Migrations
{
    public partial class InicialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "config");

            migrationBuilder.EnsureSchema(
                name: "common");

            migrationBuilder.EnsureSchema(
                name: "business");

            migrationBuilder.EnsureSchema(
                name: "audit");

            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.CreateTable(
                name: "Countries",
                schema: "common",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsoCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentType",
                schema: "common",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FacilityType",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Create = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MembershipPlan",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BillingCycle = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    MaxUsers = table.Column<int>(type: "int", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipPlan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Module",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Module", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsSystemRole = table.Column<bool>(type: "bit", nullable: false),
                    IsSuperUser = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                schema: "common",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Department_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "common",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TradeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: true),
                    DocumentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    VerificationDigit = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Subdomain = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Create = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalSchema: "common",
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permission_Module_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "config",
                        principalTable: "Module",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlanModule",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MembershipPlanId = table.Column<int>(type: "int", nullable: false),
                    ModuleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanModule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanModule_MembershipPlan_MembershipPlanId",
                        column: x => x.MembershipPlanId,
                        principalSchema: "config",
                        principalTable: "MembershipPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanModule_Module_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "config",
                        principalTable: "Module",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "City",
                schema: "common",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.Id);
                    table.ForeignKey(
                        name: "FK_City_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "common",
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationCompanies",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ApplicationCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LoginField = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Create = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationCompanies_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanySubscription",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    MembershipPlanId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AutoRenew = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanySubscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanySubscription_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanySubscription_MembershipPlan_MembershipPlanId",
                        column: x => x.MembershipPlanId,
                        principalSchema: "config",
                        principalTable: "MembershipPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Facility",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    FacilityTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Create = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Facility_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Facility_FacilityType_FacilityTypeId",
                        column: x => x.FacilityTypeId,
                        principalSchema: "business",
                        principalTable: "FacilityType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsersInfo",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AuthProvider = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ConfirmedAccount = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersInfo_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "business",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersInfo_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalSchema: "common",
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersInfo_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "config",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    IsGranted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermission_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "config",
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermission_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "config",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacilityAddress",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    PlaceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    IsMain = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityAddress_City_CityId",
                        column: x => x.CityId,
                        principalSchema: "common",
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacilityAddress_Facility_FacilityId",
                        column: x => x.FacilityId,
                        principalSchema: "business",
                        principalTable: "Facility",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetToken",
                schema: "audit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetToken_UsersInfo_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "UsersInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                schema: "audit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginField = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    JwtId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AccessTokenExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_UsersInfo_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "UsersInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAddress",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserInfoId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    PlaceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    IsMain = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAddress_City_CityId",
                        column: x => x.CityId,
                        principalSchema: "common",
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAddress_UsersInfo_UserInfoId",
                        column: x => x.UserInfoId,
                        principalSchema: "auth",
                        principalTable: "UsersInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSession",
                schema: "audit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginField = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeviceFingerprint = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LoginAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogoutAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSession_UsersInfo_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "UsersInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationCompanies_CompanyId",
                schema: "config",
                table: "ApplicationCompanies",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_City_DepartmentId",
                schema: "common",
                table: "City",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_DocumentTypeId",
                schema: "business",
                table: "Companies",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanySubscription_CompanyId",
                schema: "config",
                table: "CompanySubscription",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanySubscription_MembershipPlanId",
                schema: "config",
                table: "CompanySubscription",
                column: "MembershipPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_CountryId",
                schema: "common",
                table: "Department",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Facility_CompanyId",
                schema: "business",
                table: "Facility",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Facility_FacilityTypeId",
                schema: "business",
                table: "Facility",
                column: "FacilityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityAddress_CityId",
                schema: "business",
                table: "FacilityAddress",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityAddress_FacilityId",
                schema: "business",
                table: "FacilityAddress",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetToken_UserId",
                schema: "audit",
                table: "PasswordResetToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_ModuleId",
                schema: "config",
                table: "Permission",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanModule_MembershipPlanId",
                schema: "config",
                table: "PlanModule",
                column: "MembershipPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanModule_ModuleId",
                schema: "config",
                table: "PlanModule",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId",
                schema: "audit",
                table: "RefreshToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionId",
                schema: "config",
                table: "RolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_RoleId",
                schema: "config",
                table: "RolePermission",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddress_CityId",
                schema: "auth",
                table: "UserAddress",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddress_UserInfoId",
                schema: "auth",
                table: "UserAddress",
                column: "UserInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_UserId",
                schema: "audit",
                table: "UserSession",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInfo_CompanyId",
                schema: "auth",
                table: "UsersInfo",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInfo_DocumentTypeId",
                schema: "auth",
                table: "UsersInfo",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInfo_RoleId",
                schema: "auth",
                table: "UsersInfo",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationCompanies",
                schema: "config");

            migrationBuilder.DropTable(
                name: "CompanySubscription",
                schema: "config");

            migrationBuilder.DropTable(
                name: "FacilityAddress",
                schema: "business");

            migrationBuilder.DropTable(
                name: "PasswordResetToken",
                schema: "audit");

            migrationBuilder.DropTable(
                name: "PlanModule",
                schema: "config");

            migrationBuilder.DropTable(
                name: "RefreshToken",
                schema: "audit");

            migrationBuilder.DropTable(
                name: "RolePermission",
                schema: "config");

            migrationBuilder.DropTable(
                name: "UserAddress",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "UserSession",
                schema: "audit");

            migrationBuilder.DropTable(
                name: "Facility",
                schema: "business");

            migrationBuilder.DropTable(
                name: "MembershipPlan",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Permission",
                schema: "config");

            migrationBuilder.DropTable(
                name: "City",
                schema: "common");

            migrationBuilder.DropTable(
                name: "UsersInfo",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "FacilityType",
                schema: "business");

            migrationBuilder.DropTable(
                name: "Module",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Department",
                schema: "common");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "business");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Countries",
                schema: "common");

            migrationBuilder.DropTable(
                name: "DocumentType",
                schema: "common");
        }
    }
}
