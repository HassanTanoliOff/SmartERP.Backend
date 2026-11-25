namespace Core.Entities;

public class Warehouse
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public List<Stock> Stocks { get; set; } = new();
}
