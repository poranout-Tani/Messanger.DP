using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Applications.Interface.Repositories
{
    public interface IMessagesRepository
    {
        Task<List<Messages>> GetByChat(Guid chatId);
        Task<List<Messages>> GetByFilter(Guid chatId, string? searchtext = null);

        Task AddMessages(Messages message);

        Task MessageIsRead(Guid Id);

        Task UpdateMessage(Guid Id, string textmessage, DateTime timestamp);

        Task DeleteMessage(Guid Id);

    }
}
