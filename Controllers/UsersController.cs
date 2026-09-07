using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TimeRecord.Data;
using TimeRecord.DTO.Auth;
using TimeRecord.DTO.Login;
using TimeRecord.Services;

namespace TimeRecord.Controllers
{
    [ApiController]
    [Route("api/auth/")]
    [SwaggerTag("Authentication and management of access users.")]
    public class UsersController(UserService userService) : ControllerBase
    {
        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Authenticates a user.",
            Description = "Validates email and password and returns a JWT access token valid for 12 hours.")]
        [ProducesResponseType(typeof(TimeRecord.Models.Token), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ValidateUserAsync(
            [SwaggerRequestBody("Login credentials (email and password).")] LoginDto requestLoginDto)
        {
            var validatedUser = await userService.LoginUserToken(requestLoginDto.Email, requestLoginDto.Password);
            return Ok(validatedUser);
        }
        
        [HttpPost("register/employee")]
        [SwaggerOperation(
            Summary = "Registers an employee user.",
            Description = "Creates the access user and its employee profile. The email must be unique and the informed company user must have the admin role.")]
        [ProducesResponseType(typeof(UsersResponseTokenDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEmployeeAsync(
            [SwaggerRequestBody("User data (email, password, roles) and employee data (name, job, matriculation, company id).")]
            RegisterEmployeeDto requestLoginEmployeeEmployeeDto)
        {
            var userEmployeeCreated = await userService.CreateUserEmployeeAsync(requestLoginEmployeeEmployeeDto);
            return Ok(userEmployeeCreated);
        }

        [HttpPost("register/companies")]
        [SwaggerOperation(
            Summary = "Registers a company user.",
            Description = "Creates the access user and its company profile. The email must be unique.")]
        [ProducesResponseType(typeof(UsersResponseTokenDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCompaniesAsync(
            [SwaggerRequestBody("User data (email, password, roles) and the company name.")]
            RegisterComapiesDto requestLoginEmployeeCompaniesDto)
        {
            var userCompaniesCreated = await userService.CreatUserCompaniesAsync(requestLoginEmployeeCompaniesDto);
            return Ok(userCompaniesCreated);
        }
        

        [HttpGet("users")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Lists the users in the system.",
            Description = "Returns all registered access users. Requires a JWT token.")]
        [ProducesResponseType(typeof(IEnumerable<TimeRecord.Models.Users>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAsync()
        {
            var allUsers = await userService.GetUserAsync();
            return Ok(allUsers);
        }

        [HttpPut("users/{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Updates a user.",
            Description = "Updates the email and password of the user. Requires a JWT token.")]
        [ProducesResponseType(typeof(UsersResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync(
            [SwaggerRequestBody("New user data (email and password).")] LoginDto dataDto,
            [SwaggerParameter("User id.")] int id)
        {
            var updatedUser = await userService.UpdateUserAsync(dataDto, id);
            return Ok(updatedUser);
        }

        [HttpDelete("users/{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Deletes a user.",
            Description = "Deletes the access user and returns a confirmation message. Requires a JWT token.")]
        [ProducesResponseType(typeof(UsersMessageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(
            [SwaggerParameter("User id.")] int id)
        {
            var deletedUser = await userService.DeleteUserAsync(id);
            return Ok(deletedUser);
        }


       
        [HttpGet("test/token")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Checks the token.",
            Description = "Returns whether the sent JWT token is valid and authenticated.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Test()
        {
            return Ok(new
            {
                isAuth = User.Identity?.IsAuthenticated
            });
        }
    }
}
