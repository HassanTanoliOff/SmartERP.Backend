using Core.Enums;

namespace Core.Entities;

public class PurchaseOrder
{
    public int Id { get; set; }
    public string PONumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    // Changed from DateTime → DateOnly (SRS/data types: plain order date, no time)
    public DateOnly OrderDate { get; set; }
    public PurchaseOrderStatus Status { get; set; } // Draft / Approved / Received / Cancelled
    public decimal TotalAmount { get; set; }
    public int CreatedBy { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public User CreatedByUser { get; set; } = null!;
    public User? ApprovedByUser { get; set; }
    public Supplier Supplier { get; set; } = null!;
}
