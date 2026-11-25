using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntitiesConfigurations;

public class SalesOrderEntityConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.ToTable("SalesOrders");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.SalesOrderNumber).IsUnique();
        builder.Property(x => x.SalesOrderNumber).IsRequired().HasMaxLength(30);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.SubTotal).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Discount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2).IsRequired();
        // Changed: OrderDate mapped as date (was DateTime / no date column type)
        builder.Property(x => x.OrderDate).HasColumnType("date").IsRequired();
        // Changed from CURRENT_DATE → now() (CreatedAt is a UTC timestamp, not a plain date)
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("now()");

        builder
            .HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(y => y.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(y => y.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
