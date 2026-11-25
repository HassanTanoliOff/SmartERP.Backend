using Core.Enums;

namespace Core.Entities;

public class SalesOrder
{
    public int Id { get; set; }
    public string SalesOrderNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int WarehouseId { get; set; }
    // Changed from DateTime → DateOnly (SRS/data types: plain order date, no time)
    public DateOnly OrderDate { get; set; }
    public SalesOrderStatus Status { get; set; } = SalesOrderStatus.NotSet;
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Customer Customer { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;
}
