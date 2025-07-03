using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Data.DTO
{
    public class PharmacyRegisterDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        
        [Phone]
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(01)[0-2,5]{1}[0-9]{8}$", ErrorMessage = "Please enter a valid  phone number starting with 01 & contain 11 number")]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
