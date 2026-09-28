using Applications.Interface.Repositories;
using Core.Contracts.DTO_s;
using System;
using System.Collections.Generic;
using System.Text;

namespace Applications.Service
{
    public class ChatMembersServices
    {
        private readonly IChatMembersRepository _chatMembersRepository;
        public ChatMembersServices(IChatMembersRepository chatMembersRepository)
        {
            _chatMembersRepository = chatMembersRepository;
        }

        public async Task<List<UserResponse>> GetChatMembersAsync(Guid chatId)
        {
            var members = await _chatMembersRepository.GetAllMembers(chatId);

            return members.Select(cm => new UserResponse
            {
                ID = cm.UserID,
                Username = cm.User?.Username ?? "Незнакомец",
                AvatarURL = null,
                LastSeen = DateTime.UtcNow
            }).ToList();
        }

        public async Task AddMemberAsync(Guid chatId, Guid userId)
        {
            await _chatMembersRepository.AddMember(chatId, userId);
        }

        public async Task DeleteMemberAsync(Guid chatId, Guid userId)
        {
            await _chatMembersRepository.DeleteMember(chatId, userId);
        }

        public async Task<bool> IsChatMemberAsync(Guid chatId, Guid userId)
        {
            return await _chatMembersRepository.IsMember(chatId, userId);
        }
        public async Task UpdateMemberRoleAsync(Guid chatId, Guid userId, string newRole)
        {
            await _chatMembersRepository.UpdateMember(chatId, userId, newRole);
        }

    }
}
