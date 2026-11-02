namespace Core.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SKU { get; set; }
    public string Description { get; set; }
    public int Category_Id { get; set; }
    public Category Category { get; set; }
    public decimal Unit_Price { get; set; }
    public decimal Cost_Price { get; set; }
    public string Unit { get; set; } // unit of measurement eg { pack, box , Kg, grams }
    public string ReOrder_Level { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime Created_At { get; set; }
    public DateTime Updated_At { get; set; }
}
