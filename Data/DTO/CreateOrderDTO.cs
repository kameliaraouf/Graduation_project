using System.ComponentModel.DataAnnotations;
namespace GraduationProject.Data.DTO
{
    public class CreateOrderDTO
    {

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Governorate { get; set; }
    }
}
