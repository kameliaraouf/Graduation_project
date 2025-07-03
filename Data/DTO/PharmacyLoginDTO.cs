using GraduationProject.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.DTO
{
    public class PharmacyLoginDTO
    {
       

        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
      
    }
}
