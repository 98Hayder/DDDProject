using DDDProject.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DDDProject.Infrastructure.Configurations
{
    public class SeedDataConfiguration : IEntityTypeConfiguration<UserRole>, IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasData(
                new UserRole { Id = 1, Role = "Admin" },
                new UserRole { Id = 2, Role = "Customer" }
            );
        }

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasData(
                new User
                {
                    Id = 1,
                    UserName = "Admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin"),
                    FullName = "Admin",
                    RoleID = 1,
                    Created = DateTime.Now
                },
                new User
                {
                    Id = 2,
                    UserName = "Customer",
                    Password = BCrypt.Net.BCrypt.HashPassword("Customer"),
                    FullName = "Customer",
                    RoleID = 2,
                    Created = DateTime.Now
                }
            );
        }
    }
}
