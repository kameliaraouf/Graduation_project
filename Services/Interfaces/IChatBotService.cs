using GraduationProject.Data.DTO;
using GraduationProject.Data.DTOs;
using GraduationProject.Data.Entities;
using GraduationProject.Data.Models;
using GraduationProject.Services.Interfaces;
namespace GraduationProject.Services
{ 
    public interface IChatBotService
    {
        Task<QuestionResponseDTO> CreateQuestionAsync(int userId, CreateQuestionDTO questionDto);
        Task<ConversationHistorySummaryDTO> GetAllConversationsAsync(int userId);
        Task<ConversationDTO> GetConversationAsync(string conversationIdentifier, int userId);
        Task DeleteConversationAsync(string conversationIdentifier, int userId);
        Task ClearAllConversationsAsync(int userId);
        
        Task HandleUserLoginAsync(int userId);
    }
}