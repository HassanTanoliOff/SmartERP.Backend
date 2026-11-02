using System.Runtime.InteropServices.JavaScript;

namespace Core.Entities;

public class PurchaseOrder
{
    public int Id { get; set; }
    public int Supplier_Id { get; set; }
    public DateTime Order_Date { get; set; }
    public string Status { get; set; }
    public decimal Total_Amount { get; set; }
    public string Created_By { get; set; }
    public string Approved_By { get; set; }
    public DateTime Created_At { get; set; }
}
