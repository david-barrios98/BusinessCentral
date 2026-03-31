using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessCentral.Domain.Entities.Auth;

namespace BusinessCentral.Infrastructure.Persistence.Configuration.Table
{
    public class UsersInfoConfigurations : IEntityTypeConfiguration<UsersInfo>
    {
        public void Configure(EntityTypeBuilder<UsersInfo> entity)
        {

            entity.Property(e => e.Active)
                .HasColumnName("active")
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.Create)
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("create");

            entity.Property(e => e.Update)
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("update");


        }
    }
}
