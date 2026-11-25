using Core.Enums;

namespace Core.Entities;

public class Product
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public decimal UnitPrice { get; set; } = 0;
    public decimal CostPrice { get; set; } = 0;
    public UnitOfMeasure Unit { get; set; } // unit of measurement eg { pack, box , Kg, grams }
    public int ReOrderLevel { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<Stock> Stocks { get; set; } = [];
}
