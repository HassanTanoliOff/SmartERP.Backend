namespace Core.Entities;

public class Invoice
{
    public int Id { get; set; }
    public string Invoice_Number { get; set; }
    public int Sales_Order_Id { get; set; }
    public DateTime Issue_Date { get; set; }
    public DateTime Due_Date { get; set; }
    public decimal Total_Amount { get; set; }
    public string Status { get; set; }
    public SalesOrder Sales_Order { get; set; }
}
