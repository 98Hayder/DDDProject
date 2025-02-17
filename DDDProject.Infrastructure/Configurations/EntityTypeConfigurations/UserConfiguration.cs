using DDDProject.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DDDProject.Infrastructure.Configurations.EntityTypeConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {        
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.UserName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.Password)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(u => u.FullName)
                   .HasMaxLength(255);

            builder.Property(u => u.RoleID)
                   .IsRequired();

            builder.Property(u => u.Created)
                    .HasDefaultValue(DateTime.Now)
                   .IsRequired();


            builder.HasOne(a => a.UserRole)
                   .WithMany()
                   .HasForeignKey(a => a.RoleID);
        }
    }
}
