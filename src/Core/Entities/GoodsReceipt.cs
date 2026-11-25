namespace Core.Entities;

public class GoodsReceipt
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }
    public int WarehouseId { get; set; }
    public int ReceivedBy { get; set; }
    public DateTime ReceivedAt { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public User ReceivedByUser { get; set; } = null!;
}
