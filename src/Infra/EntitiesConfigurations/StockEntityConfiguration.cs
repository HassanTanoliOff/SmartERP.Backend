using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntitiesConfigurations;

public class StockEntityConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.ToTable(
            "Stock",
            s => s.HasCheckConstraint("Ck_Stock_Quantity", "\"Quantity\" >= 0")
        );
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasOne(x => x.Product).WithMany(x => x.Stocks).HasForeignKey(x => x.ProductId);
        builder.HasOne(x => x.Warehouse).WithMany(x => x.Stocks).HasForeignKey(x => x.WarehouseId);

        builder.HasIndex(x => new { x.WarehouseId, x.ProductId }).IsUnique();
    }
}
