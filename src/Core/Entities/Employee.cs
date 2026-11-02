namespace Core.Entities;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int Department_Id { get; set; }
    public string Designation { get; set; }
    public DateTime Hire_Date { get; set; }
    public decimal Salary { get; set; }
    public bool Is_Active { get; set; }
    public Department Department { get; set; }
}
