using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntitiesConfigurations;

public class GoodsReceiptEntityConfiguration : IEntityTypeConfiguration<GoodsReceipt>
{
    public void Configure(EntityTypeBuilder<GoodsReceipt> builder)
    {
        builder.ToTable("GoodsReceipts");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.PurchaseOrderId).IsUnique();
        builder.Property(x => x.ReceivedAt).HasDefaultValueSql("now()");

        builder.HasOne(x => x.Warehouse).WithMany().HasForeignKey(x => x.WarehouseId);
        builder
            .HasOne(x => x.PurchaseOrder)
            .WithOne()
            .HasForeignKey<GoodsReceipt>(x => x.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(x => x.ReceivedByUser)
            .WithMany()
            .HasForeignKey(x => x.ReceivedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
