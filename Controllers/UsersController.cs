using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeRecord.Data;
using TimeRecord.DTO.Login;
using TimeRecord.Services;

namespace TimeRecord.Controllers
{
    [ApiController]
    [Route("api/auth/")]
    public class UsersController(UserService userService, AppDbContext appDbContext) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> ValidateUserAsync(LoginDto requestLoginDto)
        {
            var validatedUser = await userService.LoginUserToken(requestLoginDto.Email, requestLoginDto.PasswordHash);
            return Ok(validatedUser);
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> CreateAsync(CreateUserDto requestLoginDto)
        {
            var userCreated = await userService.CreateUserAsync(requestLoginDto);
            return Ok(userCreated);
        }


        [HttpGet("users")]
        [Authorize]
        public async Task<IActionResult> GetAsync()
        {
            var allUsers = await userService.GetUserAsync();
            return Ok(allUsers);
        }

        [HttpPut("users/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAsync(LoginDto dataDto, int id)
        {
            var updatedUser = await userService.UpdateUserAsync(dataDto, id);
            return Ok(updatedUser);
        }

        [HttpDelete("users/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deletedUser = await userService.DeleteUserAsync(id);
            return Ok(deletedUser);
        }


       
        [HttpGet("test/token")]
        [Authorize]
        public IActionResult Test()
        {
            return Ok(new
            {
                isAuth = User.Identity?.IsAuthenticated
            });
        }
    }
}