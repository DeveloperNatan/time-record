using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TimeRecord.DTO.Employee;
using TimeRecord.Services;
using Microsoft.AspNetCore.Authorization;

namespace TimeRecord.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController(EmployeeService employeeService) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAsync(EmployeeCreateAndUpdateDto createAndUpdateRequestDto)
        {
            // var userId = int.Parse(User.FindFirstValue("userId")!);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var createdEmployee = await employeeService.CreateEmployeeAsync(createAndUpdateRequestDto,  userId);
            return Ok(createdEmployee);
        }
        
    
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllAsync()
        {
            var employees = await employeeService.GetAllUsersAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetAsync(int id)
        {
            var employee = await employeeService.GetUserAsync(id);
            return Ok(employee);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deletedEmployee = await employeeService.DeleteUserAsync(id);
            return Ok(deletedEmployee);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAsync(EmployeeCreateAndUpdateDto createAndUpdateRequestDto, int id)
        {
            var editedEmployee = await employeeService.UpdateUserAsync(createAndUpdateRequestDto, id);
            return Ok(editedEmployee);
        }

        [HttpGet("{id}/markings")]
        [Authorize]
        public async Task<IActionResult> GetMarkingAsync(int id)
        {
            var markingsEmployee = await employeeService.GetMarkingUserAsync(id);
            return Ok(markingsEmployee);
        }
    }
}