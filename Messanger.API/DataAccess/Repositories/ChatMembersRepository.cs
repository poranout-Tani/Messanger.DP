using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Applications.Interface.Repositories;

namespace DataAccess.Repositories
{
    public class ChatMembersRepository : IChatMembersRepository
    {
        private readonly MessangerDbContexts _dbContexts;

        public ChatMembersRepository( MessangerDbContexts dbContexts)
        {
            _dbContexts = dbContexts;
        }

        public async Task<List<ChatMembers>> GetAllMembers(Guid chatId)
        {
            return await _dbContexts.ChatMembers
                .AsNoTracking()
                .Where(cm => cm.ChatID == chatId)
                .Include(cm => cm.User)
                .ToListAsync();
        }

        public async Task AddMember(Guid chatId, Guid userId, string role = "Участник")
        {
            var member = new ChatMembers
            {
                ChatID = chatId,
                UserID = userId,
                Role = role,
                JoinedAt = DateTime.UtcNow
            };

            await _dbContexts.ChatMembers.AddAsync(member);
            await _dbContexts.SaveChangesAsync();
        }

        public async Task UpdateMember(Guid chatId, Guid userId, string newRole)
        {
            await _dbContexts.ChatMembers
                .Where(cm => cm.ChatID == chatId && cm.UserID == userId)
                .ExecuteUpdateAsync(s => s
                .SetProperty(cm => cm.Role, newRole));
        }

        public async Task DeleteMember(Guid chatId,Guid userId)            
        {
            await _dbContexts.ChatMembers
                .Where(cm => cm.UserID == userId && cm.ChatID == chatId)
                .ExecuteDeleteAsync();
        }

        public async Task<bool> IsMember(Guid chatId, Guid userId)
        {
            return await _dbContexts.ChatMembers
                .AnyAsync(cm => cm.ChatID == chatId && cm.UserID == userId);
            
        }

    }
}
