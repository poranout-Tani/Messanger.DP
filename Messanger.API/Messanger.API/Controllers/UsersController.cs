using Core.Models;
using DataAccess.Repositories;
using Core.Contracts.DTO_s;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.OpenApi;

namespace Messanger.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UsersRepository _usersRepository;

        public UsersController(UsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _usersRepository.Get();
        var response = users.Select(u => new UserResponse
        {
            ID = u.ID,
            Username = u.Username,
            AvatarURL = u.AvatarURL,
            LastSeen = u.LastSeen
        }).ToList();

        return Ok(response);
    }

    [HttpGet("filter")]
        public async Task<IActionResult> GetByFilter([FromQuery] string title)
        {
            var users = await _usersRepository.GetByFilter(title);
            var response = users.Select(u => new UserResponse
            {
                ID = u.ID,
                Username = u.Username,
                AvatarURL = u.AvatarURL,
                LastSeen = u.LastSeen
            }).ToList();

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserRegisterRequest newUser)
        {
            var newId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var user = new Users
            {
                ID = newId,
                Username = newUser.Username,
                PasswordHash = newUser.Password,
                AvatarURL = newUser.AvatarURL,
                CreatedAt = now,
                LastSeen = now
            };

            //await _usersRepository.Add(
            //    newId,
            //    newUser.Username,
            //    newUser.Password,
            //    newUser.AvatarURL,
            //    now,
            //    now
            //    );
            return Ok(new {id = newId});
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequest updatedUser)
        //{
        //    var existingUser = await _usersRepository.GetById(id);
        //    if (existingUser == null) return NotFound();

        //    string passwordToSave = !string.IsNullOrWhiteSpace(updatedUser.Password)
        //        ? updatedUser.Password
        //        : existingUser.PasswordHash;

        //    await _usersRepository.Update(
        //        id,
        //        updatedUser.Username,
        //        updatedUser.AvatarURL,
        //        DateTime.UtcNow,
        //        passwordToSave,
        //        existingUser.CreatedAt);

        //    return Ok();
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _usersRepository.Delete(id);
            return Ok();
        }

    }
}
