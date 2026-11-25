using Core.Enums;

namespace Core.Entities;

public class Payment
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }
    // Changed from DateTime → DateOnly (SRS/data types: plain payment date, when money was received)
    public DateOnly PaymentDate { get; set; }
    public PaymentMethods Method { get; set; } = PaymentMethods.Unset;
    public int RecordedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Invoice Invoice { get; set; } = null!;
    public User RecordedByUser { get; set; } = null!;
}
