using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraduationProject.Data.Entities;
using GraduationProject.Data.Models;

namespace GraduationProject.Data.Repositories
    {
    public interface IChatBotRepository
    {
        Task<(Conversation conversation, Message message)> SaveQuestionAsync(int userId, string question, string response);
        Task<List<Conversation>> GetUserConversationsAsync(int userId);
        Task<Conversation> GetConversationAsync(Guid conversationId, int userId);
        Task DeleteConversationAsync(Guid conversationId, int userId);
        Task ClearAllConversationsAsync(int userId);
        Task<ChatBotHistory> GetOrCreateChatHistoryAsync(int userId);



    }
}

