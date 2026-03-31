using BusinessCentral.Domain.Entities.Audit;
using BusinessCentral.Domain.Entities.Auth;
using BusinessCentral.Domain.Entities.Business;
using BusinessCentral.Domain.Entities.Common;
using BusinessCentral.Domain.Entities.Config;
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

        // Esquema AUTH
        public DbSet<UsersInfo> Users { get; set; } = null!;
        public DbSet<UserSession> UserSessions { get; set; } = null!;


        // Esquema BUSINESS
        public DbSet<Companies> Companies { get; set; } = null!;
        public DbSet<FacilityType> FacilityType { get; set; } = null!;
        public DbSet<Facility> Facility { get; set; } = null!;
        public DbSet<FacilityAddress> FacilityAddress { get; set; } = null!;


        // Esquema COMMON
        public DbSet<Country> Country { get; set; } = null!;
        public DbSet<Department> Department { get; set; } = null!;
        public DbSet<DocumentType> DocumentTypes { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;


        // Esquema CONFIG
        public DbSet<ApplicationCompanies> ApplicationCompanies { get; set; } = null!;
        public DbSet<CompanySubscription> CompanySubscription { get; set; } = null!;
        public DbSet<MembershipPlan> MembershipPlan { get; set; } = null!;
        public DbSet<Module> Modules { get; set; } = null!;
        public DbSet<Permission> Permission { get; set; } = null!;
        public DbSet<PlanModule> PlanModule { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;
        public DbSet<RolePermission> RolePermissions { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Carga automática de configuraciones si decides hacer alguna manual
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BusinessCentralDbContext).Assembly);

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

                // --- EL RESTO DE TU LÓGICA (CASCADA, ETC) ---
                foreach (var fk in entity.GetForeignKeys())
                {
                    fk.DeleteBehavior = DeleteBehavior.Restrict;
                }

                // --- OPCIONAL: AUTO-SNAKE_CASE PARA COLUMNAS ---
                // Si quieres que en la DB se llamen "user_id" y en C# "UserId"
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