using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntitiesConfigurations;

public class PurchaseOrderEntityConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrders");
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.PONumber).IsUnique();
        builder.Property(p => p.PONumber).IsRequired().HasMaxLength(30);
        builder.Property(p => p.Status).IsRequired().HasConversion<string>().HasMaxLength(50);
        builder.Property(p => p.TotalAmount).IsRequired().HasPrecision(18, 2);
        // Changed: OrderDate mapped as date (was DateTime / no date column type)
        builder.Property(p => p.OrderDate).HasColumnType("date").IsRequired();
        // Changed from CURRENT_DATE → now() (CreatedAt is a UTC timestamp, not a plain date)
        builder.Property(p => p.CreatedAt).HasDefaultValueSql("now()");

        builder
            .HasOne(p => p.Supplier)
            .WithMany()
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(p => p.CreatedByUser)
            .WithMany()
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(p => p.ApprovedByUser)
            .WithMany()
            .HasForeignKey(p => p.ApprovedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
