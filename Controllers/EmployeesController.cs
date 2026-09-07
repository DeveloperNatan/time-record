using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TimeRecord.DTO.Employee;
using TimeRecord.DTO.Markings;
using TimeRecord.Services;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace TimeRecord.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Management of employees. All endpoints require a JWT token.")]
    public class EmployeesController(EmployeeService employeeService) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        [SwaggerOperation(
            Summary = "Creates an employee.",
            Description = "Creates an employee linked to the user id taken from the JWT token. The name must be unique.")]
        [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateAsync(
            [SwaggerRequestBody("Employee data (name and job).")] EmployeeCreateAndUpdateDto createAndUpdateRequestDto)
        {
            // var userId = int.Parse(User.FindFirstValue("userId")!);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var createdEmployee = await employeeService.CreateEmployeeAsync(createAndUpdateRequestDto,  userId);
            return Ok(createdEmployee);
        }
        
    
        [HttpGet]
        [Authorize]
        [SwaggerOperation(
            Summary = "Lists the employees in the system.",
            Description = "Returns all registered employees.")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllAsync()
        {
            var employees = await employeeService.GetAllUsersAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Lists one employee.",
            Description = "Returns the employee that owns the given matriculation.")]
        [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync(
            [SwaggerParameter("Employee matriculation.")] int id)
        {
            var employee = await employeeService.GetUserAsync(id);
            return Ok(employee);
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Deletes an employee.",
            Description = "Deletes the employee and returns a confirmation message.")]
        [ProducesResponseType(typeof(EmployeeMessageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(
            [SwaggerParameter("Employee id.")] int id)
        {
            var deletedEmployee = await employeeService.DeleteUserAsync(id);
            return Ok(deletedEmployee);
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Updates an employee.",
            Description = "Updates the employee data and returns the altered employee.")]
        [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync(
            [SwaggerRequestBody("New employee data.")] EmployeeCreateAndUpdateDto createAndUpdateRequestDto,
            [SwaggerParameter("Employee id.")] int id)
        {
            var editedEmployee = await employeeService.UpdateUserAsync(createAndUpdateRequestDto, id);
            return Ok(editedEmployee);
        }

        [HttpGet("{id}/markings")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Lists the time markings of an employee.",
            Description = "Returns every time marking registered for the given employee.")]
        [ProducesResponseType(typeof(IEnumerable<TimeRecordsResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMarkingAsync(
            [SwaggerParameter("Employee id.")] int id)
        {
            var markingsEmployee = await employeeService.GetMarkingUserAsync(id);
            return Ok(markingsEmployee);
        }
    }
}
