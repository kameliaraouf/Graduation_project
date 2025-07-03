using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Data.Entities
{
    public class ChatBotHistory
    {
        [Key]
        public int Id { get; set; }

      


        [ForeignKey("Conversation")]
        public Guid ConversationId { get; set; }

        public virtual ICollection<Conversation> Conversations { get; set; }

        [Required]
        public int UserID { get; set; }

       public virtual User User { get; set; } // Navigation property
    }
}