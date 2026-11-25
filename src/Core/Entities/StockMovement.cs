using Core.Enums;

namespace Core.Entities;

public class StockMovement
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int Quantity { get; set; }
    public StockMovementType MovementType { get; set; } // Recipet / Sale / Adjustment Transfer out / transfer In
    public StockMovementReferenceTypes ReferenceType { get; set; }
    public int ReferenceId { get; set; }
    public string? ReasonAdjustment { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;
}
