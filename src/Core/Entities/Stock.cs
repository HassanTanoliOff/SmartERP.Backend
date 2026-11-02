namespace Core.Entities;

public class Stock
{
    public int Id { get; set; }
    public int Product_Id { get; set; }
    public int Quantity { get; set; }
    public Product Product { get; set; }
}
