using DataAccess;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using Core.Models;
using Core.Contracts.DTO_s;
using Core.Contracts.Messages;
using Core.Contracts.Users;

namespace Messanger.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatsController : ControllerBase
    {
        private readonly ChatsRepository _chatsRepository;

        public ChatsController(ChatsRepository chatsRepository)
        {
            _chatsRepository = chatsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllChats()
        {
            var chat = await _chatsRepository.GetChats();
            var response = chat.Select(c => new ChatResponse
            {
                ID = c.ID,
                ChatName = c.ChatName,
                TimeCreate = c.TimeCreate
            }).ToList();

            return Ok(response);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetChatUsers(Guid userId)
        {
            var chat = await _chatsRepository.GetUserChats(userId);
            var response = chat.Select(c => new ChatResponse
            {
                ID = c.ID,
                ChatName = c.ChatName,
                TimeCreate = c.TimeCreate
            }).ToList();

            return Ok(response);
        }

        [HttpGet("{id}/full")]
        public async Task<IActionResult> GetFullInfoChat(Guid id)
        {
            var chat = await _chatsRepository.GetFullInfoChats(id);            
            if (chat == null) return NotFound();

            var respone = new ChatDetailsResponse
            {
                ID = chat.ID,
                ChatName = chat.ChatName,

                Messages = chat.Messages.Select(m => new MessagesResponce(
                    m.ID,                           
                    m.TextMessage,                  
                    m.Sender?.Username ?? "System", 
                    m.SenderID,
                    m.Timestamp,                    
                    false                           
                    // , chat.ID                    раскомментируй, если уже добавил его в контракт
                )).ToList(),

                Members = chat.ChatMembers.Select(cm => new UserResponse 
                {
                    ID = cm.User.ID,
                    Username = cm.User.Username,
                    LastSeen = DateTime.UtcNow,
                }).ToList()
            };

            return Ok(respone);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] ChatCreateRequest newChat)
        {
            var newChatId = Guid.NewGuid();
            var timestamp = DateTime.UtcNow;

            var chatEntity = new Core.Models.Chats
            {
                ID = newChatId,
                ChatName = newChat.ChatName,
                InterlocutorId = newChat.InterlocutorId
            };
            await _chatsRepository.CreateChat(chatEntity);

            return Ok(new {id = newChatId, time = timestamp});
        }

        [HttpPut("{id}/name")]
        public async Task<IActionResult> UpdateChat(Guid id ,[FromBody] ChatCreateRequest updateName)
        {
            await _chatsRepository.UptadeChat(
                id,
                updateName.ChatName,
                updateName.InterlocutorId
                );
            return Ok();            
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _chatsRepository.DeleteChat(id);
            return Ok();
        }

    }
}
