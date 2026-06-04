using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TimeRecord.Data;
using TimeRecord.Validation;
using TimeRecord.DTO.Employee;
using TimeRecord.DTO.Markings;
using TimeRecord.Exceptions;
using TimeRecord.Models;

namespace TimeRecord.Services
{
    public class EmployeeService(AppDbContext appDbContext)
    {
        public async Task<EmployeeResponseDto> CreateEmployeeAsync(EmployeeCreateAndUpdateDto dataDto, int userId)
        {
            EmployeeValidator.Validate(dataDto);
            var userExists = await appDbContext.Users.AnyAsync(u => u.Id == userId);
            if(!userExists)
            {
                throw new NotFoundException(404, "Matriculation not exist");
            }
            var exisingEmployee = await appDbContext.Employees.AnyAsync(e => e.Name == dataDto.Name);


            if (exisingEmployee)
            {
                throw new ValidationException("This name already exists, try another");
            }


            var createdEmployee = new Employee()
            {
                Name = dataDto.Name,
                Job = dataDto.Job,
                UserId = userId ,
            };

            await appDbContext.Employees.AddAsync(createdEmployee);
            await appDbContext.SaveChangesAsync();

            var response = new EmployeeResponseDto()
            {
                UserId = createdEmployee.Id,
                Name = createdEmployee.Name,
            };
            return response;
        }

        public async Task<IEnumerable<EmployeeResponseDto>> GetAllUsersAsync()
        {
            var employees = await appDbContext.Employees.ToListAsync();
            if (!employees.Any())
            {
                throw new NotFoundException(404, "Matriculation not found in the system!");
            }

            var response = employees.Select(employee => new EmployeeResponseDto()
            {
                UserId = employee.Id,
                Name = employee.Name,
                Matriculation = employee.Matriculation,
                Job = employee.Name,
            });

            return response;
        }

        public async Task<EmployeeResponseDto> GetUserAsync(int matriculation)
        {
            var employee = await appDbContext.Employees.FindAsync(matriculation);
            //puxar pela matricula
            var teste = await appDbContext.Employees.FirstOrDefaultAsync(e => e.Matriculation == matriculation);
            if (teste == null)
            {
                throw new NotFoundException(404, "Matriculation not found in the system!");
            }

            return new EmployeeResponseDto
            {
                UserId = teste.Id,
                Matriculation = teste.Id,
                Name = teste.Name,
                Job = teste.Name,
            };
        }

        public async Task<EmployeeMessageDto> DeleteUserAsync(int id)
        {
            var deletedEmployee = await appDbContext.Employees.FindAsync(id);
            if (deletedEmployee == null)
            {
                throw new NotFoundException(404, "Matriculation not found in the system!");
            }

            appDbContext.Remove(deletedEmployee);
            await appDbContext.SaveChangesAsync();

            var response = new EmployeeMessageDto()
            {
                Messsage = $"User {deletedEmployee.Name} has been deleted successfully!"
            };
            return response;
        }

        public async Task<EmployeeResponseDto> UpdateUserAsync(EmployeeCreateAndUpdateDto dataDto, int id)
        {
            var updatedEmployee = await appDbContext
                .Employees.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (updatedEmployee == null)
            {
                throw new NotFoundException(404, "Matriculation not found in the system!");
            }

            EmployeeValidator.Validate(dataDto);
            updatedEmployee.Name = dataDto.Name;


            await appDbContext.SaveChangesAsync();
            return new EmployeeResponseDto
            {
                UserId = updatedEmployee.Id,
                Name = updatedEmployee.Name,
            };
        }

        public async Task<IEnumerable<TimeRecordsResponseDto>> GetMarkingUserAsync(int id)
        {
            var markingsEmployee = await appDbContext.Employees.FindAsync(id);
            if (markingsEmployee == null)
            {
                throw new NotFoundException(404, "Employee ID not found in the system!");
            }

            var markings = await appDbContext
                .TimeRecords.Where(m => m.EmployeeId == id)
                .ToListAsync();

            if (markings.Count == 0)
            {
                throw new KeyNotFoundException("No time markings found for this employee!");
            }

            var response = markings.Select(employeeMarking => new TimeRecordsResponseDto()
            {
                Id = employeeMarking.Id,
                Matriculation = markingsEmployee.Matriculation,
                RecordedAt = employeeMarking.RecordedAt,
            });

            return response;
        }
    }
}