using GraduationProject.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Conversation
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("ChatBotHistory")]
    public int ChatHistoryId { get; set; }
    public virtual ChatBotHistory ChatHistory { get; set; }

    // Fix: This should be a collection of Message, not ChatBotHistory
    public virtual ICollection<Message> Messages { get; set; }
}
