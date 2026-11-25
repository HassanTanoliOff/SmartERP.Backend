using Core.Enums;

namespace Core.Entities;

public class Attendance
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    // Changed from DateTime → DateOnly (SRS/data types: plain attendance date, no time)
    public DateOnly Date { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Absent;
    public Employee Employee { get; set; } = null!;
}
