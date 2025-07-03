using GraduationProject.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Data.DTO
{
    public class UpdateProductDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public IFormFile ImageFile { get; set; }

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
