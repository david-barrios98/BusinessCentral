using BusinessCentral.Domain.Entities.Audit;
using BusinessCentral.Domain.Entities.Auth;
using BusinessCentral.Domain.Entities.Business;
using BusinessCentral.Domain.Entities.Commerce;
using BusinessCentral.Domain.Entities.Farm;
using BusinessCentral.Domain.Entities.Hr;
using BusinessCentral.Domain.Entities.Common;
using BusinessCentral.Domain.Entities.Config;
using BusinessCentral.Domain.Entities.Services;
using BusinessCentral.Domain.Entities.Finance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BusinessCentral.Infrastructure.Persistence.Adapters
{
    public class BusinessCentralDbContext : DbContext
    {
        public BusinessCentralDbContext(DbContextOptions<BusinessCentralDbContext> options)
            : base(options)
        {
        }
        // Esquema AUDIT
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<UserAddress> UserAddresses { get; set; } = null!;
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;


        // Esquema AUTH
        public DbSet<UsersInfo> Users { get; set; } = null!;
        public DbSet<UserSession> UserSessions { get; set; } = null!;


        // Esquema BUSINESS
        public DbSet<Companies> Companies { get; set; } = null!;
        public DbSet<FacilityType> FacilityType { get; set; } = null!;
        public DbSet<Facility> Facility { get; set; } = null!;
        public DbSet<FacilityAddress> FacilityAddress { get; set; } = null!;
        public DbSet<StorageLocation> StorageLocations { get; set; } = null!;


        // Esquema COMMON
        public DbSet<Countries> Country { get; set; } = null!;
        public DbSet<Department> Department { get; set; } = null!;
        public DbSet<DocumentType> DocumentTypes { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;


        // Esquema CONFIG
        public DbSet<ApplicationCompanies> ApplicationCompanies { get; set; } = null!;
        public DbSet<CompanySubscription> CompanySubscription { get; set; } = null!;
        public DbSet<MembershipPlan> MembershipPlan { get; set; } = null!;
        public DbSet<Module> Modules { get; set; } = null!;
        public DbSet<CompanyModule> CompanyModules { get; set; } = null!;
        public DbSet<BusinessNature> BusinessNatures { get; set; } = null!;
        public DbSet<BusinessNatureModule> BusinessNatureModules { get; set; } = null!;
        public DbSet<Permission> Permission { get; set; } = null!;
        public DbSet<PlanModule> PlanModule { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;
        public DbSet<RolePermission> RolePermissions { get; set; } = null!;

        // Esquema HR
        public DbSet<EmployeeProfile> EmployeeProfiles { get; set; } = null!;
        public DbSet<PayScheme> PaySchemes { get; set; } = null!;
        public DbSet<WorkLog> WorkLogs { get; set; } = null!;
        public DbSet<LoanAdvance> LoanAdvances { get; set; } = null!;
        public DbSet<Deduction> Deductions { get; set; } = null!;

        // Esquema FARM
        public DbSet<FarmZone> FarmZones { get; set; } = null!;
        public DbSet<HarvestLot> HarvestLots { get; set; } = null!;
        public DbSet<CoffeeProcessStep> CoffeeProcessSteps { get; set; } = null!;
        public DbSet<CoffeeProductFormType> CoffeeProductFormTypes { get; set; } = null!;
        public DbSet<CoffeeProcessType> CoffeeProcessTypes { get; set; } = null!;

        // Esquema SERVICES
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Vehicle> Vehicles { get; set; } = null!;
        public DbSet<ServiceCatalog> ServiceCatalog { get; set; } = null!;
        public DbSet<ServiceOrder> ServiceOrders { get; set; } = null!;
        public DbSet<ServiceOrderLine> ServiceOrderLines { get; set; } = null!;

        // Esquema COMMERCE/POS
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<ProductAttribute> ProductAttributes { get; set; } = null!;
        public DbSet<ProductAttributeOption> ProductAttributeOptions { get; set; } = null!;
        public DbSet<ProductVariant> ProductVariants { get; set; } = null!;
        public DbSet<ProductVariantOption> ProductVariantOptions { get; set; } = null!;
        public DbSet<PurchaseReceipt> PurchaseReceipts { get; set; } = null!;
        public DbSet<PurchaseReceiptLine> PurchaseReceiptLines { get; set; } = null!;
        public DbSet<InventoryMovement> InventoryMovements { get; set; } = null!;
        public DbSet<CashSession> CashSessions { get; set; } = null!;
        public DbSet<PosTicket> PosTickets { get; set; } = null!;
        public DbSet<PosTicketLine> PosTicketLines { get; set; } = null!;
        public DbSet<PosPayment> PosPayments { get; set; } = null!;

        // Esquema FIN (finanzas / reportes contables)
        public DbSet<FinancialTransaction> FinancialTransactions { get; set; } = null!;
        public DbSet<TaxConcept> TaxConcepts { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<JournalEntry> JournalEntries { get; set; } = null!;
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Carga automática de configuraciones si decides hacer alguna manual
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BusinessCentralDbContext).Assembly);

            // Claves compuestas / constraints necesarias para migraciones
            modelBuilder.Entity<CompanyModule>()
                .HasKey(x => new { x.CompanyId, x.ModuleId });

            modelBuilder.Entity<BusinessNatureModule>()
                .HasKey(x => new { x.BusinessNatureId, x.ModuleId });

            modelBuilder.Entity<ProductCategory>()
                .HasKey(x => new { x.CompanyId, x.ProductId, x.CategoryId });

            modelBuilder.Entity<ProductVariantOption>()
                .HasKey(x => new { x.CompanyId, x.VariantId, x.AttributeId, x.OptionId });

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var namespaceName = entity.ClrType.Namespace ?? "";

                // --- ASIGNACIÓN AUTOMÁTICA DE ESQUEMAS ---
                // Ponemos 'Config' primero para que no sea absorbido por 'Business' 
                // si el namespace contiene ambos nombres.
                if (namespaceName.Contains("Config"))
                    entity.SetSchema("config");
                else if (namespaceName.Contains("Auth"))
                    entity.SetSchema("auth");
                else if (namespaceName.Contains("Audit"))
                    entity.SetSchema("audit");
                else if (namespaceName.Contains("Common"))
                    entity.SetSchema("common");
                else if (namespaceName.Contains("Business"))
                    entity.SetSchema("business");
                else if (namespaceName.Contains(".Hr"))
                    entity.SetSchema("hr");
                else if (namespaceName.Contains(".Farm"))
                    entity.SetSchema("farm");
                else if (namespaceName.Contains(".Services"))
                    entity.SetSchema("svc");
                else if (namespaceName.Contains(".Commerce"))
                    entity.SetSchema("com");
                else if (namespaceName.Contains(".Finance"))
                    entity.SetSchema("fin");

                // --- EL RESTO DE TU LÓGICA (CASCADA, ETC) ---
                foreach (var fk in entity.GetForeignKeys())
                {
                    fk.DeleteBehavior = DeleteBehavior.Restrict;
                }

                // --- OPCIONAL: AUTO-SNAKE_CASE PARA COLUMNAS ---
                // Si quieres que en la DB se llamen "UserId" y en C# "UserId"
                foreach (var property in entity.GetProperties())
                {
                    // Solo si no definiste el [Column("nombre")] manualmente
                    var storeObjectIdentifier = StoreObjectIdentifier.Table(entity.GetTableName(), entity.GetSchema());
                    if (property.GetColumnName(storeObjectIdentifier) == property.Name)
                    {
                        // Aquí podrías usar una función para convertir CamelCase a SnakeCase
                    }
                }
            }
        }
    }
}