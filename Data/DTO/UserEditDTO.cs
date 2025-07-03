using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace GraduationProject.Data.DTO
{
    public class UserEditDTO
    {

        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
       
        public string ?FirstName { get; set; }

        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
       
        public string ?LastName { get; set; }


        [RegularExpression(@"^(01)[0-2,5]{1}[0-9]{8}$", ErrorMessage = "Please enter a valid  phone number starting with 01 & contain 11 number")]
       
        public string ?Phone { get; set; }


        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
       
        public string ?Email { get; set; }


        [Required(ErrorMessage = "Current password is required")]
        public string Password { get; set; }



        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
         ErrorMessage = "Password must include at least one uppercase letter, one lowercase letter, one number, and one special character")]
    
        public string? NewPassword { get; set; }
        
    }
}
