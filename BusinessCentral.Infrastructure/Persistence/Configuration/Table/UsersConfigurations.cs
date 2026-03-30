using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessCentral.Domain.Entities.Auth;

namespace BusinessCentral.Infrastructure.Persistence.Configuration.Table
{
    public class UsersConfigurations : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> entity)
        {

            entity.Property(e => e.active)
                .HasColumnName("active")
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.create)
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("create");

            entity.Property(e => e.update)
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("update");


        }
    }
}
