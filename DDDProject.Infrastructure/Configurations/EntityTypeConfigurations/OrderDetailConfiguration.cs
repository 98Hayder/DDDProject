using DDDProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDDProject.Domain.Configurations.EntityTypeConfigurations
{
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.ToTable("OrderDetails");

            builder.HasKey(od => od.Id);

            builder.Property(od => od.Quantity)
                .IsRequired();

            builder.Property(od => od.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(od => od.TotalPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            

            builder.HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);

            builder.HasOne(od => od.Book)
                .WithMany()
                .HasForeignKey(od => od.BookId);
        }
    }
}
