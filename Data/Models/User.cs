using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Data.Entities
{
    public class User
    {
        public int UserID { get; set; }
        [Required]
        [StringLength(20)]
        public string UserName { get; set; }
        [Required]
        public string Password { get;set; }
        
        public string FirstName { get; set; }
     
        public string LastName { get; set; }
      
        public string Phone { get; set; }
    
        public string Email { get; set; }
        [Required]
        public string Role { get; set; }

       

        public virtual ICollection<Order> Orders { get; set; }
        public virtual Cart Cart { get; set; }
        public virtual ChatBotHistory ChatBotHistory { get; set; }
        
        public virtual ICollection<Diagnosis> Diagnosis { get; set; }

    }
}
