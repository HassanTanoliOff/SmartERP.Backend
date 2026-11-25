using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntitiesConfigurations;

public class StockMovementEntityConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable(
            "StockMovements",
            t =>
            {
                t.HasCheckConstraint("ck_stock_movement_quantity", "\"Quantity\" <> 0");
                t.HasCheckConstraint(
                    "ck_stock_movement_reason",
                    "\"MovementType\" <> 'Adjustment' OR \"ReasonAdjustment\" IS NOT NULL "
                );
            }
        );
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.MovementType).IsRequired().HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.ReferenceType).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.ReasonAdjustment).HasMaxLength(300);
        builder.Property(x => x.CreatedBy).IsRequired();
        builder.Property(x => x.CreatedDate).HasDefaultValueSql("now()");

        builder
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new
        {
            x.ProductId,
            x.WarehouseId,
            x.CreatedDate,
        });

        builder.HasIndex(x => new { x.ReferenceType, x.ReferenceId });
    }
}
