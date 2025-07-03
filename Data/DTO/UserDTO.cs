using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace GraduationProject.Data.DTO
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
   
        public string Email { get; set; }
        
    }
}
