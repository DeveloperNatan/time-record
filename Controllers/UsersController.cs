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
        
        [HttpPost("register/employee")]
        public async Task<IActionResult> CreateEmployeeAsync(RegisterEmployeeDto requestLoginEmployeeEmployeeDto)
        {
            var userEmployeeCreated = await userService.CreateUserEmployeeAsync(requestLoginEmployeeEmployeeDto);
            return Ok(userEmployeeCreated);
        }

        [HttpPost("register/companies")]
        public async Task<IActionResult> CreateCompaniesAsync(RegisterComapiesDto requestLoginEmployeeCompaniesDto)
        {
            var userCompaniesCreated = await userService.CreatUserCompaniesAsync(requestLoginEmployeeCompaniesDto);
            return Ok(userCompaniesCreated);
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