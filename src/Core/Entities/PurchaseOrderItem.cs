namespace Core.Entities;

public class PurchaseOrderItem
{
    public int Id { get; set; }
    public int Purchase_Order_Id { get; set; }
    public int Product_Id { get; set; }
    public int Quantity { get; set; }
    public decimal Unit_Cost { get; set; }
    public PurchaseOrder Purchase_Order { get; set; }
}
