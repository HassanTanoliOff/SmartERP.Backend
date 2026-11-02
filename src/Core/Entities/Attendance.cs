namespace Core.Entities;

public class Attendance
{
    public int Id { get; set; }
    public int Employee_Id { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; }
    public Employee Employee { get; set; }
}
