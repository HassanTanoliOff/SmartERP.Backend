using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntitiesConfigurations;

public class PurchaseOrderItemEntityConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable(
            "PurchaseOrderItems",
            t =>
            {
                t.HasCheckConstraint("ck_product_quantity", "\"Quantity\" >= 1");
            }
        );
        builder.HasKey(pi => pi.Id);
        builder.Property(pi => pi.ProductId).IsRequired();
        builder.Property(pi => pi.PurchaseOrderId).IsRequired();
        builder.Property(pi => pi.Quantity).IsRequired();
        builder.Property(pi => pi.UnitCost).IsRequired().HasPrecision(18, 2);

        builder.HasIndex(pi => new { pi.ProductId, pi.PurchaseOrderId }).IsUnique();

        builder
            .HasOne(pi => pi.PurchaseOrder)
            .WithMany()
            .HasForeignKey(pi => pi.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(pi => pi.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
