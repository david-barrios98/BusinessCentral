using Microsoft.EntityFrameworkCore;
using BusinessCentral.Infrastructure.Persistence.Configuration.Table;

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
            modelBuilder.ApplyConfiguration(new UsersConfigurations());

            base.OnModelCreating(modelBuilder);
        }
    }
}