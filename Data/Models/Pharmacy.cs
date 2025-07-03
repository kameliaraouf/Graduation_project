using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Data.Entities
{
    public class Pharmacy
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }
        public string HashPassword { get; set; }
        public string saltPassword { get; set; }
        public string role { get; set; }


        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public ICollection<ProductPharmacy> productPharmacy { get; set; }
        public ICollection<Order> PharmacyOrder { get; set; }
    
    }
}
