using DDDProject.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DDDProject.Domain.Configurations.EntityTypeConfigurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRole");
            builder.HasKey(ur => ur.Id);

            builder.Property(ur => ur.Role).IsRequired().HasMaxLength(100);
        }
    }
}
