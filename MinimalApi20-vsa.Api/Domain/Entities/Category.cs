namespace MinimalApi20_vsa.Api.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Optional: Add relation to products if needed
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
