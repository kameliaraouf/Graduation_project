using System.ComponentModel.DataAnnotations;
namespace GraduationProject.Data.DTO
{
    public class MessageRequestDTO
    {
       
            [Required]
            public Guid ConversationId { get; set; }

            [Required]
            public string Question { get; set; }
        
    }
}
