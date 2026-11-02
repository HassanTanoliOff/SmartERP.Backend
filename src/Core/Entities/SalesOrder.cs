namespace Core.Entities;

public class SalesOrder
{
    public int Id { get; set; }
    public int Customer_Id { get; set; }
    public DateTime Order_Date { get; set; }
}
