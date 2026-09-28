using System;
using System.Collections.Generic;
using System.Text;
using Core.Models;
using Applications.Interface.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class ChatsRepository : IChatsRepository
    {
        private readonly MessangerDbContexts _dbContext;

        public ChatsRepository(MessangerDbContexts dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Chats>> GetChats()
        {
            return await _dbContext.Chats
                .AsNoTracking()
                .OrderBy(c => c.ChatName)
                .ToListAsync();
        }

        public async Task CreateChat(Chats chat)
        {
            //var Chat = new Chats
            //{
            //    ID = chat.ID,
            //    InterlocutorId = chat.InterlocutorId,
            //    ChatName = chat.ChatName,
            //    UsersID = chat.UsersID,
            //};

            //await _dbContext.Chats.AddAsync(chat);
            //await _dbContext.SaveChangesAsync();

            var userExists = await _dbContext.Users.AnyAsync(u => u.ID == chat.UsersID);
            if (!userExists)
                throw new Exception($"Пользователь {chat.UsersID} не найден");

            await _dbContext.Chats.AddAsync(chat);
            await _dbContext.SaveChangesAsync();

        }

        public async Task<Chats?> GetPrivateChat(Guid interlocutorId)
        {
            return await _dbContext.Chats
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.InterlocutorId == interlocutorId);
        }

        public async Task<Chats?> GetFullInfoChats(Guid id)
        {

            return await _dbContext.Chats
                .AsNoTracking()
                .Include(c => c.Messages) 
                .ThenInclude(m => m.Sender)
                .Include(c => c.ChatMembers)
                .ThenInclude(cm => cm.User)
                .FirstOrDefaultAsync(c => c.ID == id);
        }       

        public async Task<List<Chats>> GetUserChats(Guid userId)
        {
            return await _dbContext.Chats
                .AsNoTracking()
                .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
                .Include(c => c.ChatMembers)
                .ThenInclude(cm => cm.User)
                .Where(c => c.UsersID == userId || c.InterlocutorId == userId || _dbContext.ChatMembers.Any(cm => cm.ChatID == c.ID && cm.UserID == userId))
                .OrderByDescending(c => c.TimeCreate)
                .ToListAsync();
        }


        public async Task UptadeChat(Guid id, string newchatName, Guid? inclocutorId)
        {
            await _dbContext.Chats
               .Where(c => c.ID == id)
               .ExecuteUpdateAsync(s => s
               .SetProperty(c => c.ChatName, newchatName)
               .SetProperty(c => c.InterlocutorId, inclocutorId));
        }

        public async Task DeleteChat(Guid id)
        {
            await _dbContext.Chats
                .Where(c => c.ID == id)
                .ExecuteDeleteAsync();
        }

    }
}
