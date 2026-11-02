namespace Core.Entities;

public class SalesOrderItem
{
    public int Id { get; set; }
    public int Sales_Order_Id { get; set; }
    public int Product_Id { get; set; }
    public int Quantity { get; set; }
    public decimal Unit_Price { get; set; }
    public SalesOrder Sales_Order { get; set; }
    public Product Product { get; set; }
}
