using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntitiesConfigurations;

public class AuditLogEntityConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Action).IsRequired().HasMaxLength(50);
        builder.Property(x => x.EntityName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.EntityId).IsRequired();
        builder.Property(x => x.OldValues).HasMaxLength(300);
        builder.Property(x => x.NewValues).HasMaxLength(300);
        builder.Property(x => x.Timestamp).HasDefaultValueSql("now()");

        builder.HasIndex(x => new
        {
            x.UserId,
            x.EntityName,
            x.Timestamp,
        });

        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
    }
}
