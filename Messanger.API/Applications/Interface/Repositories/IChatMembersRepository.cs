using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Applications.Interface.Repositories
{
    public interface IChatMembersRepository
    {
        Task<List<ChatMembers>> GetAllMembers(Guid chatId);
        Task<bool> IsMember(Guid chatId, Guid userId);

        Task AddMember(Guid chatId, Guid userId, string role = "Участник");

        Task UpdateMember(Guid chatId, Guid userId, string newRole);

        Task DeleteMember(Guid chatId, Guid userId);
    }
}
