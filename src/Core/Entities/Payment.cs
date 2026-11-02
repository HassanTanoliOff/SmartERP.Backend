namespace Core.Entities;

public class Payment
{
    public int Id { get; set; }
    public int Invoice_Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Payment_Date { get; set; }
    public string Payment_Method { get; set; }
    public Invoice Invoice { get; set; }
}
