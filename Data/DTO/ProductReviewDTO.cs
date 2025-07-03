using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Data.DTO
{
    public class ProductReviewDTO
    {


        [Required]
        public string ProductName { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public double Rating { get; set; }

        [Required]
        public string ReviewText { get; set; }
    }
}


