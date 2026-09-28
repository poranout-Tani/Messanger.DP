using System;
using System.Collections.Generic;
using System.Text;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Applications.Interface.Repositories;

namespace DataAccess.Repositories
{
    public class MessagesRepository : IMessagesRepository
    {
        private readonly MessangerDbContexts _dbContext;
       
        public MessagesRepository(MessangerDbContexts dbContexts)
        {
            _dbContext = dbContexts;
        }
        
        public async Task<List<Messages>> GetByChat(Guid chatId)
        {
            return await _dbContext.Messages
                .AsNoTracking()
                .Include(m => m.Sender)
                .Where(m => m.ChatID == chatId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task<List<Messages>> GetByFilter(Guid chatId, string? searchtext = null)
        {
            var query = _dbContext.Messages
                .AsNoTracking()
                .Include(m => m.Sender)
                .Where(m => m.ChatID == chatId);

            if (!string.IsNullOrWhiteSpace(searchtext))
            {
                query = query.Where(m => m.TextMessage.Contains(searchtext));
            }

            return await query
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }



        public async Task AddMessages(Messages message)
        {
            var Message = new Messages
            {
                ID = message.ID,
                ChatID = message.ChatID,
                SenderID = message.SenderID,
                TextMessage = message.TextMessage,
                Timestamp = message.Timestamp,
            };

            await _dbContext.Messages.AddAsync(Message);
            await _dbContext.SaveChangesAsync();
        }

        public async Task MessageIsRead(Guid Id)
        {
            await _dbContext.Messages
                .Where(m => m.ID == Id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(m => m.IsRead, true));
        }

        public async Task UpdateMessage(Guid Id, string textmessage, DateTime timestamp)
        {
            await _dbContext.Messages
                .Where(m => m.ID == Id)
                .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.TextMessage, textmessage)
                .SetProperty(m => m.Timestamp, timestamp));       
        }

        public async Task DeleteMessage(Guid Id)
        {
            await _dbContext.Messages
                .Where(m => m.ID == Id)
                .ExecuteDeleteAsync();
        }

    }
}
