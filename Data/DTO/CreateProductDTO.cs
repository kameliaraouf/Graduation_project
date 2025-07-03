using GraduationProject.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GraduationProject.Data.DTO
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProductCategoryDto
    {
        SkincareProduct,
        HerbalProduct,
        Medicine
    }
    public class CreateProductDTO
    {
        [Required]
       
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; }  // This should match what you're sending in Postman

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required]
        public ProductCategory Category { get; set; }
    }
}
