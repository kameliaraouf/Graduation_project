using System.ComponentModel.DataAnnotations;
namespace GraduationProject.Data.DTO
{
    public class DeleteAccountDTO
    {
        [Required]
        public string Password { get; set; }
    }
}
