using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using TikTok_Clone_User_Service.DatabaseContext;
using TikTok_Clone_User_Service.Models;

namespace TikTok_Clone_User_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DbUserContext _dbContext;

        public UserController(DbUserContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetuserbyId(int usersId) { 
            var user =  await _dbContext.Users.FindAsync(usersId);
            if (user == null) { return NotFound(); }

            return Ok(user);

        }

        [HttpPost]
        public async Task<IActionResult> createUser([FromBody]  UserDto userDto)
        {

            //check if user exist 
            var VLUser = await _dbContext.Users.FirstAsync(u => u.Auth_id == userDto.AuthId);
            if(VLUser != null) { return Ok("user is already in db") ; };

            string role = "user";
            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Auth_id = userDto.AuthId,
                Role = role
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> updateUser(int userId, string email, string username, string description)
        {
            try
            {

                var user = await _dbContext.Users.FindAsync(userId);
                if (user == null) { return NotFound("The user could not be found"); };

             

                if (email != null)
                {
                    user.Email = email;
                }

                if (username != null)
                {
                    user.Name = username;
                }
                if (description != null)
                {
                    user.Description = description;
                }

                _dbContext.Update(user);

                await _dbContext.SaveChangesAsync();

                return Ok(user);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> deleteUser(int userId)
        {
            if(userId == 0) { return NotFound(); }
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) { return NotFound(); };

             _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return Ok("User deleted successfully.");

        }
    }

    
}   
