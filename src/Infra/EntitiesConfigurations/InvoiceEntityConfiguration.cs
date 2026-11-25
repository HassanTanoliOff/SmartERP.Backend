using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntitiesConfigurations;

public class InvoiceEntityConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.InvoiceNumber).IsUnique();
        builder.Property(x => x.InvoiceNumber).IsRequired().HasMaxLength(30);
        builder.HasIndex(x => x.SalesOrderId).IsUnique();
        builder.Property(x => x.SalesOrderId).IsRequired();
        builder.Property(x => x.TotalAmount).IsRequired().HasPrecision(18, 2);
        // Changed: IssueDate/DueDate are DateOnly now; due date set in app (issue + payment terms), not fixed SQL default
        builder.Property(x => x.IssueDate).HasColumnType("date").IsRequired();
        builder.Property(x => x.DueDate).HasColumnType("date").IsRequired();
        builder.Property(x => x.Status).IsRequired().HasConversion<string>().HasMaxLength(50);

        builder
            .HasOne(x => x.SalesOrder)
            .WithOne()
            .HasForeignKey<Invoice>(x => x.SalesOrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
