using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntitiesConfigurations;

public class AttendanceEntityConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.ToTable("Attendance");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EmployeeId).IsRequired();
        // Changed from datetime + CURRENT_TIMESTAMP default → date, no default
        // (SRS: plain attendance date set by HR input; entity DateTime → DateOnly)
        builder.Property(x => x.Date).HasColumnType("date").IsRequired();
        builder.Property(x => x.Status).IsRequired().HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(x => new { x.EmployeeId, x.Date }).IsUnique();

        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId);
    }
}
