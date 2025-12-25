using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CreateProductDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    
    [Required]
    public string PictureUrl { get; set; } = string.Empty;

    [Required]
    public string Brand { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "QuantityInStock is required")]
    [Range(1, int.MaxValue, ErrorMessage = "QuantityInStock must be greater than 0")]
    public int QuantityInStock { get; set; }
}
