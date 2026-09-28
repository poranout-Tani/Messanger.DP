using Applications.Interface.Repositories;
using Core.Models;
using DataAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly MessangerDbContexts _dbContext;

        public UsersRepository(MessangerDbContexts dbContexts)
        {
            _dbContext = dbContexts;
        }

        public async Task<Users> GetByName(string Username)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == Username);
            return user;
        }

        public async Task<List<Users>> Get()
        {
            return await _dbContext.Users
                .AsNoTracking()
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<List<Users>> GetWithMessages()
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Messages)
                .ToListAsync();
        }

        public async Task<Users?> GetById(Guid id)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ID == id);
        }

        public async Task<List<Users>> GetByFilter(string title)
        {
            var query = _dbContext.Users.AsNoTracking();

            if (!string .IsNullOrEmpty(title))
            {
                query = query.Where(u => u.Username.ToLower().Contains(title.ToLower()));
            }

            return await query.ToListAsync();
        }

        public async Task Add(Users user)
            //Guid id, string username, string password, string? avatarURL = null, DateTime? lastseen = null, DateTime? createdAt = null)
        {
            //var User = new Users
            //{
            //    ID = id,
            //   Username = username,
            //    PasswordHash = password,
            //    AvatarURL = avatarURL,
            //    LastSeen = lastseen ?? DateTime.UtcNow,
            //    CreatedAt = createdAt ?? DateTime.UtcNow
            var User = new Users
            {
                ID = user.ID,
                Username = user.Username,
                PasswordHash = user.PasswordHash,
                AvatarURL = user.AvatarURL,
                LastSeen = user.LastSeen,
                CreatedAt = user.CreatedAt

            };

            await _dbContext.Users.AddAsync(User);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Guid id, string username, string? avatarURL, DateTime lastseen, string password, string? bio, string? bday)
        {
            await _dbContext.Users
                .Where(u => u.ID == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.Username, username)
                    .SetProperty(u => u.AvatarURL, avatarURL)
                    .SetProperty(u => u.LastSeen, lastseen)
                    .SetProperty(u => u.PasswordHash, password)
                    .SetProperty(u => u.Bio, bio)
                    .SetProperty(u => u.Bday, bday));
        }

        public async Task UpdatePassword(Guid id, string newPassword)
        {
            await _dbContext.Users
                .Where(u => u.ID == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.PasswordHash, newPassword)
                    .SetProperty(u => u.LastSeen, DateTime.UtcNow));
        }

        public async Task Delete(Guid id)
        {
            await _dbContext.Users
                .Where(u => u.ID == id)
                .ExecuteDeleteAsync();
        }

    }
}
