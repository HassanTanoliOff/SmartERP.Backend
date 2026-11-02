namespace Core.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public int User_Id { get; set; }
    public string Action { get; set; }
    public string Entity_Name { get; set; }
    public string Entity_ID { get; set; }
    public DateTime Timestamp { get; set; }
}
