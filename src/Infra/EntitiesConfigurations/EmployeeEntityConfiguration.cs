using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntitiesConfigurations;

public class EmployeeEntityConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable(
            "Employees",
            t =>
            {
                t.HasCheckConstraint("ck_salary", "\"Salary\" >0");
            }
        );
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(150);
        builder.HasIndex(e => e.Email).IsUnique();
        builder.Property(e => e.Email).IsRequired().HasMaxLength(150);
        builder.Property(e => e.Phone).HasMaxLength(30);
        builder.Property(e => e.DepartmentId).IsRequired();
        builder.Property(e => e.Designation).IsRequired().HasMaxLength(100);
        // HireDate stays date; entity changed DateTime → DateOnly
        builder.Property(e => e.HireDate).HasColumnType("date").IsRequired();
        builder.Property(e => e.Salary).IsRequired().HasPrecision(18, 2);
        // Changed from CURRENT_DATE → now() (CreatedAt is a UTC timestamp, not a plain date)
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

        builder.HasOne(e => e.Department).WithMany().HasForeignKey(e => e.DepartmentId);
    }
}
