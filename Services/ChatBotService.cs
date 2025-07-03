using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GraduationProject.Data.DTOs;
using GraduationProject.Data.Entities;
using GraduationProject.Data.Models;
using GraduationProject.Data.Repositories;
using GraduationProject.Repositories;
using Microsoft.Extensions.Options;

namespace GraduationProject.Services
{
 

    public class ChatBotService : IChatBotService
    {
        private readonly IChatBotRepository _chatBotRepository;
        private readonly ChatBotSettings _chatBotSettings;
        private readonly HttpClient _httpClient;
        private readonly IChatSessionService _chatSessionService;

        public ChatBotService(
            IChatBotRepository chatBotRepository,
            IOptions<ChatBotSettings> chatBotSettings,
            HttpClient httpClient,
            IChatSessionService chatSessionService)
        {
            _chatBotRepository = chatBotRepository;
            _chatBotSettings = chatBotSettings.Value;
            _httpClient = httpClient;
            _chatSessionService = chatSessionService;
        }

        public async Task HandleUserLoginAsync(int userId)
        {
            
            _chatSessionService.EndSession(userId);
        }

        public async Task<QuestionResponseDTO> CreateQuestionAsync(int userId, CreateQuestionDTO questionDto)
        {
            
            bool isNewSession = _chatSessionService.IsNewSession(userId);

            
            _chatSessionService.MarkSessionActive(userId);

           
            string aiResponse = await GetAIResponseAsync(questionDto.Question);

            
            await _chatBotRepository.GetOrCreateChatHistoryAsync(userId);

            
            Conversation conversation;
            Message message;

            
            (conversation, message) = await _chatBotRepository.SaveQuestionAsync(
                userId,
                questionDto.Question,
                aiResponse
            );

            
            return new QuestionResponseDTO
            {
                Question = message.Question,
                Response = message.Response,
                Date = message.Date,
                ConversationIdentifier = conversation.Id.ToString()
            };
        }

        public async Task<ConversationHistorySummaryDTO> GetAllConversationsAsync(int userId)
        {
            var conversations = await _chatBotRepository.GetUserConversationsAsync(userId);

            var conversationSummaries = new List<ConversationSummaryDTO>();

            foreach (var conversation in conversations)
            {
                var firstMessage = conversation.Messages?.OrderBy(m => m.Date).FirstOrDefault();

                conversationSummaries.Add(new ConversationSummaryDTO
                {
                    ConversationIdentifier = conversation.Id.ToString(),
                    CreatedAt = conversation.CreatedAt,
                    FirstQuestion = firstMessage?.Question ?? "No messages",
                    MessageCount = conversation.Messages?.Count ?? 0
                });
            }

            return new ConversationHistorySummaryDTO
            {
                Conversations = conversationSummaries
            };
        }

        public async Task<ConversationDTO> GetConversationAsync(string conversationIdentifier, int userId)
        {
            if (!Guid.TryParse(conversationIdentifier, out Guid conversationId))
            {
                return null;
            }

            var conversation = await _chatBotRepository.GetConversationAsync(conversationId, userId);

            if (conversation == null)
            {
                return null;
            }

            var messageList = new List<MessageDTO>();

            foreach (var message in conversation.Messages.OrderBy(m => m.Date))
            {
                messageList.Add(new MessageDTO
                {
                    Question = message.Question,
                    Response = message.Response,
                    Date = message.Date
                });
            }

            return new ConversationDTO
            {
                ConversationIdentifier = conversation.Id.ToString(),
                CreatedAt = conversation.CreatedAt,
                Messages = messageList
            };
        }

        public async Task DeleteConversationAsync(string conversationIdentifier, int userId)
        {
            if (!Guid.TryParse(conversationIdentifier, out Guid conversationId))
            {
                throw new ArgumentException("Invalid conversation identifier");
            }

            await _chatBotRepository.DeleteConversationAsync(conversationId, userId);
        }

        public async Task ClearAllConversationsAsync(int userId)
        {
            await _chatBotRepository.ClearAllConversationsAsync(userId);
        }

        private async Task<string> GetAIResponseAsync(string question)
        {
            try
            {
                // Set up the API endpoint and authentication
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _chatBotSettings.GroqApiKey);

                
                var payload = new
                {
                    messages = new[]
                    {
                        new { role = "user", content = question }
                    },
                    model = "llama3-8b-8192" 
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                
                var response = await _httpClient.PostAsync(_chatBotSettings.GroqApiUrl, content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<JsonElement>(responseString);

                return responseObject
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();
            }
            catch (Exception ex)
            {
                
                return $"I apologize, but I couldn't process your request at the moment. Error: {ex.Message}";
            }
        }
    }
}