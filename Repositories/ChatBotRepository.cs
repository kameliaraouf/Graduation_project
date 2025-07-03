using GraduationProject.Data.Entities;
using GraduationProject.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraduationProject.Data.Repositories
{
    public class ChatBotRepository : IChatBotRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatBotRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ChatBotHistory> GetOrCreateChatHistoryAsync(int userId)
        {
            var chatHistory = await _context.ChatBotHistory
                .FirstOrDefaultAsync(h => h.UserID == userId);

            if (chatHistory == null)
            {
                chatHistory = new ChatBotHistory
                {
                    UserID = userId,
                    Conversations = new List<Conversation>()
                };
                _context.ChatBotHistory.Add(chatHistory);
                await _context.SaveChangesAsync();
            }

            return chatHistory;
        }

        public async Task<(Conversation conversation, Message message)> SaveQuestionAsync(int userId, string question, string response)
        {
            var chatHistory = await GetOrCreateChatHistoryAsync(userId);

           
            var lastActiveConversation = await _context.Conversations
                .Include(c => c.Messages)
                .Where(c => c.ChatHistoryId == chatHistory.Id)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync();

            Conversation conversation;

           
            bool createNewConversation = lastActiveConversation == null ||
                                         (lastActiveConversation.Messages != null &&
                                          lastActiveConversation.Messages.Any() &&
                                          (DateTime.UtcNow - lastActiveConversation.Messages.Max(m => m.Date)).TotalMinutes > 30);

          
            bool forceNewSession = true; 

            if (createNewConversation || forceNewSession)
            {
                conversation = new Conversation
                {
                    Id = Guid.NewGuid(), 
                    CreatedAt = DateTime.UtcNow,
                    ChatHistoryId = chatHistory.Id,
                    Messages = new List<Message>()
                };
                _context.Conversations.Add(conversation);
            }
            else
            {
                conversation = lastActiveConversation;
            }

            var message = new Message
            {
                Question = question,
                Response = response,
                Date = DateTime.UtcNow,
                ConversationId = conversation.Id
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return (conversation, message);
        }

        public async Task<List<Conversation>> GetUserConversationsAsync(int userId)
        {
            var chatHistory = await _context.ChatBotHistory
                .Include(ch => ch.Conversations)
                .ThenInclude(c => c.Messages)
                .FirstOrDefaultAsync(ch => ch.UserID == userId);

            if (chatHistory == null)
                return new List<Conversation>();

            // Make sure to load the full conversation history with all messages
            var conversations = await _context.Conversations
                .Include(c => c.Messages)
                .Where(c => c.ChatHistoryId == chatHistory.Id)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return conversations;
        }

        public async Task<Conversation> GetConversationAsync(Guid conversationId, int userId)
        {
            var chatHistory = await _context.ChatBotHistory
                .FirstOrDefaultAsync(ch => ch.UserID == userId);

            if (chatHistory == null)
                return null;

            // Use explicit loading to ensure all messages are included
            var conversation = await _context.Conversations
                .Include(c => c.Messages.OrderBy(m => m.Date))
                .FirstOrDefaultAsync(c => c.Id == conversationId && c.ChatHistoryId == chatHistory.Id);

            return conversation;
        }

        public async Task DeleteConversationAsync(Guid conversationId, int userId)
        {
            var chatHistory = await _context.ChatBotHistory
                .FirstOrDefaultAsync(ch => ch.UserID == userId);

            if (chatHistory == null)
                return;

            var conversation = await _context.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == conversationId && c.ChatHistoryId == chatHistory.Id);

            if (conversation == null)
                return;

            _context.Messages.RemoveRange(conversation.Messages);
            _context.Conversations.Remove(conversation);
            await _context.SaveChangesAsync();
        }

        public async Task ClearAllConversationsAsync(int userId)
        {
            var chatHistory = await _context.ChatBotHistory
                .Include(ch => ch.Conversations)
                .ThenInclude(c => c.Messages)
                .FirstOrDefaultAsync(ch => ch.UserID == userId);

            if (chatHistory == null)
                return;

            foreach (var conversation in chatHistory.Conversations)
            {
                _context.Messages.RemoveRange(conversation.Messages);
            }

            _context.Conversations.RemoveRange(chatHistory.Conversations);
            await _context.SaveChangesAsync();
        }
    }
}
       
