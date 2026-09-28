using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Applications.Interface.Repositories
{
        public interface IUsersRepository
        {

            Task<Users> GetByName(string Username);
            Task<List<Users>> Get();
            Task<List<Users>> GetWithMessages();
            Task<Users?> GetById(Guid id);
            Task<List<Users>> GetByFilter(string title);

            Task Add(Users user);   //Guid id, string username, string password, string? avatarURL = null, DateTime? lastseen = null, DateTime? createdAt = null);
                    
            Task Update(Guid id, string username, string? avatarURL, DateTime lastseen, string password, string? bio, string? bday);

            Task UpdatePassword(Guid id, string newPassword);

            Task Delete(Guid id);
        }
}
