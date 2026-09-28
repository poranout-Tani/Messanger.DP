using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Applications.Interface.Repositories
{
    public interface IChatsRepository
    {
        Task<List<Chats>> GetChats();
        Task<Chats?> GetPrivateChat(Guid interlocutorId);
        Task<Chats?> GetFullInfoChats(Guid id);
        Task<List<Chats>> GetUserChats(Guid userId);

        Task CreateChat(Chats chat);


        Task UptadeChat(Guid id, string newchatName, Guid? inclocutorId);

        Task DeleteChat(Guid id);


    }
}
