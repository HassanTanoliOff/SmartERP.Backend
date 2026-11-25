namespace Core.Entities;

public class Category
{
    public int Id { get; init; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public List<Product>? Products { get; set; } = new List<Product>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
