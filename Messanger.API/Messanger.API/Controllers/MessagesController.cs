//using Core.Models;
//using DataAccess.Repositories;
//using Core.Contracts.DTO_s;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.OpenApi;
//using System.ComponentModel.DataAnnotations;

//namespace Messanger.API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class MessagesController : ControllerBase
//    {
//        private readonly MessagesRepository _messagesRepository;

//        public MessagesController(MessagesRepository messagesRepository)
//        {
//            _messagesRepository = messagesRepository;
//        }

//        [HttpGet("chat/{chatId}")]
//        public async Task<IActionResult> GetByChat(Guid chatId)
//        {
//            var message = await _messagesRepository.GetByChat(chatId);

//            var response = message.Select(m => new MessageResponse
//            {
//                ID = m.ID,
//                TextMessage = m.TextMessage,
//                Timestamp = m.Timestamp,
//                IsRead = m.IsRead,
//                SenderID = m.SenderID,
//                SenderName = m.Sender?.Username ?? "Удалленный пользователь"
//            }).ToList();

//            return Ok(response);
//        }

//        [HttpGet("filter")]
//        public async Task<IActionResult> GetByFilter(Guid chatId, [FromQuery] string? textmessage)
//        {
//            var message = await _messagesRepository.GetByFilter(chatId, textmessage);
//            var response = message.Select(m => new MessageResponse
//            {
//                ID = m.ID,
//                TextMessage = m.TextMessage,
//                Timestamp = m.Timestamp,
//                IsRead = m.IsRead,
//                SenderID = m.SenderID,
//                SenderName = m.Sender?.Username ?? "Удаленный пользователь"
//            }).ToList();

//            return Ok(response);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create([FromBody] MessageCreateRequest newmessage)
//        {
//            var messageId = Guid.NewGuid();
//            var timestamp = DateTime.UtcNow;

//            var Message = new Messages
//            {
//                ID = messageId,
//                ChatID = newmessage.ChatID,
//                SenderID = newmessage.SenderID,
//                TextMessage = newmessage.TextMessage,
//                Timestamp = timestamp,
//            };

//            return Ok(new { id = messageId, time = timestamp });
//        }

//        [HttpPut("{Id}")]
//        public async Task<IActionResult> UpdateMessage(Guid id, [FromBody] MessageCreateRequest updatemessage)
//        {
//            await _messagesRepository.UpdateMessage(
//                id,
//                updatemessage.TextMessage,
//                DateTime.UtcNow
//                );

//            return Ok();
//        }

//        [HttpPatch("{id}/read")]
//        public async Task<IActionResult> MarkAsRead(Guid id)
//        {
//            await _messagesRepository.MessageIsRead(id);
//            return Ok();
//        }

//        [HttpDelete("{Id}")]
//        public async Task<IActionResult> DeleteMessage(Guid Id)
//        {
//            await _messagesRepository.DeleteMessage(Id);
//            return Ok();
//        }

//    }
//}
