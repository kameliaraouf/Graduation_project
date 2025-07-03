using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace GraduationProject.Data.DTO
{
    public class UpdateOrderDTO
    {
        // public int OrderID { get; set; }
        [Required]
        public string ConfirmationCode { get; set; }
     

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        [DefaultValue("string")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        [DefaultValue("string")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(01)[0-2,5]{1}[0-9]{8}$", ErrorMessage = "Please enter a valid  phone number starting with 01 & contain 11 number")]
        [DefaultValue("01205045683")]
        public string Phone { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string Governorate { get; set; }
    }
}
