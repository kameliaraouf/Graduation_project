using System;
using System.Threading.Tasks;
using GraduationProject.Data.DTOs;
using GraduationProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GraduationProject.Controllers
{
    [ApiController]
    [Authorize] 
    [Route("api/[controller]")]
    public class ChatBotController : ControllerBase
    {
        private readonly IChatBotService _chatBotService;

        public ChatBotController(IChatBotService chatBotService)
        {
            _chatBotService = chatBotService;
        }

        // Helper method to get the current user ID from claims
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("User not authenticated properly");
            }

            return userId;
        }

       
        [HttpPost("initialize-session")]
        public async Task<ActionResult> InitializeSession()
        {
            try
            {
                var userId = GetCurrentUserId();
                await _chatBotService.HandleUserLoginAsync(userId);
                return Ok(new { message = "Chat session initialized successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

     
        [HttpPost("question")]
        public async Task<ActionResult<QuestionResponseDTO>> CreateQuestion([FromBody] CreateQuestionDTO questionDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(questionDto.Question))
                {
                    return BadRequest("Question cannot be empty");
                }

                var userId = GetCurrentUserId();
                var response = await _chatBotService.CreateQuestionAsync(userId, questionDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

       
        [HttpGet("conversations")]
        public async Task<ActionResult<ConversationHistorySummaryDTO>> GetAllConversations()
        {
            try
            {
                var userId = GetCurrentUserId();
                var conversations = await _chatBotService.GetAllConversationsAsync(userId);
                return Ok(conversations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

       
        [HttpGet("conversations/{conversationId}")]
        public async Task<ActionResult<ConversationDTO>> GetConversation(string conversationId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var conversation = await _chatBotService.GetConversationAsync(conversationId, userId);

                if (conversation == null)
                {
                    return NotFound("Conversation not found");
                }

                return Ok(conversation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

     
        [HttpDelete("conversations/{conversationId}")]
        public async Task<ActionResult> DeleteConversation(string conversationId)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _chatBotService.DeleteConversationAsync(conversationId, userId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    
        [HttpDelete("conversations")]
        public async Task<ActionResult> ClearAllConversations()
        {
            try
            {
                var userId = GetCurrentUserId();
                await _chatBotService.ClearAllConversationsAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}