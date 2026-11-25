using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntitiesConfigurations;

public class SalesOrderItemEntityConfiguration : IEntityTypeConfiguration<SalesOrderItem>
{
    public void Configure(EntityTypeBuilder<SalesOrderItem> builder)
    {
        builder.ToTable(
            "SalesOrderItems",
            t =>
            {
                t.HasCheckConstraint("Ck_product_quantity", "\"Quantity\" >=1");
            }
        );
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SalesOrderId).IsRequired();
        builder.Property(x => x.ProductId).IsRequired();
        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2).IsRequired();
        builder.HasIndex(x => new { x.SalesOrderId, x.ProductId }).IsUnique();

        builder
            .HasOne(x => x.SalesOrder)
            .WithMany()
            .HasForeignKey(f => f.SalesOrderId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(f => f.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
