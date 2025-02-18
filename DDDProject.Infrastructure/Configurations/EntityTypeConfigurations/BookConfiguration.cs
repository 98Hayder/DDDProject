using DDDProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDDProject.Infrastructure.Configurations.EntityTypeConfigurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(b => b.AvailableQuantity)
                        .IsRequired();

            builder.Property(b => b.Author)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.IsAvailable)
                .HasComputedColumnSql("CASE WHEN AvailableQuantity > 0 THEN 1 ELSE 0 END");


            builder.Property(u => u.BookImage).IsRequired(false);

            builder.HasOne(b => b.Genre)
                .WithMany(g => g.Books) 
                .HasForeignKey(b => b.GenreId);       
        }
    }
}
