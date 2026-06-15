using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeRecord.Data;
using TimeRecord.DTO.Login;
using TimeRecord.Services;

namespace TimeRecord.Controllers
{
    [ApiController]
    [Route("api/auth/")]
    public class AuthenticatorController(AuthService authService, AppDbContext appDbContext) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> ValidateUserAsync(LoginDto requestLoginDto)
        {
            var validatedUser = await authService.LoginUserToken(requestLoginDto.Email, requestLoginDto.PasswordHash);
            return Ok(validatedUser);
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> CreateAsync(LoginDto requestLoginDto)
        {
            var userCreated = await authService.CreateUserAsync(requestLoginDto);
            return Ok(userCreated);
        }


        [HttpGet("users")]
        [Authorize]
        public async Task<IActionResult> GetAsync()
        {
            var allUsers = await authService.GetUserAsync();
            return Ok(allUsers);
        }

        [HttpPut("users/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAsync(LoginDto dataDto, int id)
        {
            var updatedUser = await authService.UpdateUserAsync(dataDto, id);
            return Ok(updatedUser);
        }

        [HttpDelete("users/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deletedUser = await authService.DeleteUserAsync(id);
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