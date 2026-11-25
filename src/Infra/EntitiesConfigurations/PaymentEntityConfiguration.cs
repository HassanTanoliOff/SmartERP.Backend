using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntitiesConfigurations;

public class PaymentEntityConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.InvoiceId).IsRequired();
        builder.Property(p => p.Amount).IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.RecordedBy).IsRequired();
        builder.Property(p => p.Method).IsRequired().HasConversion<string>();
        // Changed: PaymentDate entity DateTime → DateOnly; stays mapped as date (set from input, not created-at)
        builder.Property(p => p.PaymentDate).HasColumnType("date").IsRequired();
        builder.Property(p => p.CreatedAt).HasDefaultValueSql("now()");
        // Navigations and relations
        builder.HasOne(p => p.Invoice).WithMany().HasForeignKey(p => p.InvoiceId);
        builder.HasOne(p => p.RecordedByUser).WithMany().HasForeignKey(p => p.RecordedBy);
        // Indexes
        builder.HasIndex(p => p.InvoiceId);
    }
}
