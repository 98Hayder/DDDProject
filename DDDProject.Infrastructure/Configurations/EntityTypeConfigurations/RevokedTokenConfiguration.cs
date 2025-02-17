using DDDProject.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DDDProject.Infrastructure.Configurations.EntityTypeConfigurations
{
    public class RevokedTokenConfiguration : IEntityTypeConfiguration<RevokedToken>
    {
        public void Configure(EntityTypeBuilder<RevokedToken> builder)
        {
            builder.ToTable("RevokedToken");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Token)
                   .IsRequired();

            builder.Property(u => u.RevokedAt)
                    .HasDefaultValue(DateTime.Now)
                   .IsRequired();
        }

        }
    }
