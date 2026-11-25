namespace Core.Entities;

public class Stock
{
    public int Id { get; init; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int WarehouseId { get; set; }
    public Product Product { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public uint RowVersion { get; set; }
}
