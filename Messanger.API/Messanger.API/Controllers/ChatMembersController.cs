using Core.Models;
using DataAccess.Repositories;
using Core.Contracts.DTO_s;
using Microsoft.AspNetCore.Mvc;

namespace Messanger.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatMembersController : ControllerBase
    {
        private readonly ChatMembersRepository _chatMembersRepository;

        public ChatMembersController(ChatMembersRepository chatMembersRepository)
        {
            _chatMembersRepository = chatMembersRepository;
        }

        [HttpGet("chat/{chatId}")]
        public async Task<IActionResult> GetMembers(Guid chatId)
        {
            var members = await _chatMembersRepository.GetAllMembers(chatId);
            var response = members.Select(cm => new UserResponse
            {
                ID = cm.UserID,
                Username = cm.User?.Username ?? "Неизвестный",
                AvatarURL = cm.User?.AvatarURL,
                LastSeen = cm.User?.LastSeen ?? DateTime.MinValue
            }).ToList();

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddMembers([FromBody] MembersAddRequest newMember)
        {
            var isAlreadyMember = await _chatMembersRepository.IsMember(newMember.ChatID, newMember.UserID);
            if (isAlreadyMember) return BadRequest("Пользователь уже является участником чата");

            await _chatMembersRepository.AddMember(
                newMember.ChatID,
                newMember.UserID,
                newMember.Role?? "Участник"
            );

            return Ok();
        }

        [HttpPut("role")]
        public async Task<IActionResult> UpdateMembers(Guid chatId, Guid userId, [FromQuery] string newRole)
        {
            await _chatMembersRepository.UpdateMember(chatId, userId, newRole); 
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMembers(Guid chatId, Guid userId)
        {
            await _chatMembersRepository.DeleteMember(chatId, userId);
            return Ok();
        }

        [HttpGet("Check")]
        public async Task<IActionResult> CheckMembers(Guid chatId, Guid userId)
        {
            var isMemmber = await _chatMembersRepository.IsMember(chatId, userId);
            return Ok(isMemmber);
        }

    }
}
