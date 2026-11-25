using Core.Enums;

namespace Core.Entities;

public class Invoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int SalesOrderId { get; set; }
    // Changed from DateTime → DateOnly (SRS/data types: plain issue/due dates, no time)
    public DateOnly IssueDate { get; set; }
    // Changed from DateTime → DateOnly (SRS/data types: plain issue/due dates, no time)
    public DateOnly DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.NotSet;
    public SalesOrder SalesOrder { get; set; } = null!;
}
