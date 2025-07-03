using System;
using System.Collections.Generic;

namespace GraduationProject.Data.DTOs
{
    // DTO for creating a new question
    public class CreateQuestionDTO
    {
        public string Question { get; set; }
    }

    // DTO for returning conversation history
    public class ConversationDTO
    {
        public string ConversationIdentifier { get; set; } // Using a string identifier instead of exposing Guid directly
        public DateTime CreatedAt { get; set; }
        public List<MessageDTO> Messages { get; set; }
    }

    // DTO for returning conversation message details
    public class MessageDTO
    {
        public string Question { get; set; }
        public string Response { get; set; }
        public DateTime Date { get; set; }
    }

    // DTO for returning the API response of a newly created question
    public class QuestionResponseDTO
    {
        public string Question { get; set; }
        public string Response { get; set; }
        public DateTime Date { get; set; }
        public string ConversationIdentifier { get; set; }
    }

    // DTO for returning user's conversation history summary
    public class ConversationHistorySummaryDTO
    {
        public List<ConversationSummaryDTO> Conversations { get; set; }
    }

    // DTO for returning a summary of a single conversation
    public class ConversationSummaryDTO
    {
        public string ConversationIdentifier { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FirstQuestion { get; set; } // First question as a preview
        public int MessageCount { get; set; }
    }
}