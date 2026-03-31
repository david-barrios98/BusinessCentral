using Microsoft.EntityFrameworkCore;

namespace BusinessCentral.Infrastructure.Persistence.Adapters
{
    public class BusinessCentralDbContext : DbContext
    {
        public BusinessCentralDbContext(DbContextOptions<BusinessCentralDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Aplicar configuraciones manuales existentes (si las hay)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BusinessCentralDbContext).Assembly);

            // 2. Iterar sobre todas las entidades detectadas por EF
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var namespaceName = entity.ClrType.Namespace ?? "";

                // --- ASIGNACIÓN AUTOMÁTICA DE ESQUEMAS ---
                if (namespaceName.Contains("Auth")) entity.SetSchema("auth");
                else if (namespaceName.Contains("Audit")) entity.SetSchema("audit");
                else if (namespaceName.Contains("Common")) entity.SetSchema("common");
                else if (namespaceName.Contains("Business")) entity.SetSchema("business");
                else if (namespaceName.Contains("Config")) entity.SetSchema("config");

                // --- PREVENCIÓN GLOBAL DE ERRORES DE CASCADA ---
                foreach (var fk in entity.GetForeignKeys())
                {
                    fk.DeleteBehavior = DeleteBehavior.Restrict;
                }

                // --- CONVENCIÓN DE NOMBRES (Opcional: pasar de CamelCase a snake_case) ---
                // entity.SetTableName(entity.GetTableName()?.ToSnakeCase()); 
            }
        }
    }
}