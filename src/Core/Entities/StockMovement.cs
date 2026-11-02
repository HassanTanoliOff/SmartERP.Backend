namespace Core.Entities;

public class StockMovement
{
    public int Id { get; set; }
    public int Product_Id { get; set; }
    public int Warehouse_Id { get; set; }
    public int Quantity { get; set; }
    public string Movement_Type { get; set; }
    public string Reference_Type { get; set; }
    public int Reference_Id { get; set; }
    public DateTime Created_Date { get; set; }
    public DateTime Updated_Date { get; set; }
    public Warehouse Warehouse { get; set; }
    public Product Product { get; set; }
}
