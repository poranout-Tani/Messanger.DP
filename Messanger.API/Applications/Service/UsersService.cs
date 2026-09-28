using Applications.Interface.Auth;
using Applications.Interface.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Core.Models;

namespace Applications.Service
{
    public class UsersService
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUsersRepository _usersRepository;
        private readonly IJwtProvider _jwtprovider;

        public UsersService(
            IUsersRepository usersRrepository,
            IPasswordHasher passwordHasher, 
            IJwtProvider jwtProvider)
        {
            _passwordHasher = passwordHasher;
            _usersRepository = usersRrepository;
            _jwtprovider = jwtProvider;
        }

        public async Task Register(string userName, string passwordHash)
        {
            var hashedPassword = _passwordHasher.Generate(passwordHash);

            var User = Users.Create(Guid.NewGuid(), userName, hashedPassword);

            await _usersRepository.Add(User); 
        }

        public async Task<(string Token, Guid UserId)> Login(string Username, string passwordHash)
        {
            var user = await _usersRepository.GetByName(Username);

            if (user == null)
            {
                throw new Exception("Invalid username or password.");
            }

            var result = _passwordHasher.Verify(passwordHash, user.PasswordHash);

            if (result == false)
            {
                throw new Exception("Invalid username or password.");
            }

            var token = _jwtprovider.JWTGenerate(user);

            return (token, user.ID);
        }

        public async Task<Users?> GetById(Guid id)
        {
            return await _usersRepository.GetById(id);
        }

        public async Task Update(
            Guid id,
            string username,
            string? avatarUrl,
            DateTime lastSeen,
            string passwordHash,
            string? bio,
            string? bday)
        {
            // Передаем параметры дальше в репозиторий, выполняющий ExecuteUpdateAsync
            await _usersRepository.Update(id, username, avatarUrl, lastSeen, passwordHash, bio, bday);
        }

        public async Task UpdatePassword(Guid id, string newPassword)
        {
            string hashedPassword = _passwordHasher.Generate(newPassword);

            await _usersRepository.UpdatePassword(id, hashedPassword);
        }

    }
}
