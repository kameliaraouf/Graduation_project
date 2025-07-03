using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Data.DTO
{
    public class EditReviewDTO
    {
       
            [Required]
            [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
            [SwaggerParameter(Description = "Rating value between 1-5")]
            public double Rating { get; set; }

            [Required]
            [SwaggerParameter(Description = "Review text content")]
            public string ReviewText { get; set; }
        
    }
}
