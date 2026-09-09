using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TimeRecord.Data;
using TimeRecord.DTO.Users;
using TimeRecord.DTO.Login;
using TimeRecord.Exceptions;
using TimeRecord.Models;

namespace TimeRecord.Services;

public class UserService(AppDbContext appDbContext)
{
    public async Task<Token> LoginUserToken(string email, string password)
    {
        var userDb = await appDbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        if (userDb == null)
        {
            throw new NotFoundException(404, "User not found!");
        }

        bool VerifyPassword(string passwordEntered)
        {
            return BCrypt.Net.BCrypt.Verify(passwordEntered, userDb.Password);
        }

        if (!VerifyPassword(password))
        {
            throw new UnauthorizedAccessException("Password incorrect!");
        }

        var user = new UserDto()
        {
            Id = userDb.Id,
            Email = userDb.Email,
            Password = userDb.Password,
            Roles = userDb.Roles
        };

        var (token, expiresUtc) = GetToken(user);

        return new Token()
        {
            AcecessToken = token,
            TokenType = "Bearer",
            ExpiresIn = (int)(expiresUtc - DateTime.UtcNow).TotalSeconds,
        };
    }

    private (string Token, DateTime ExpiresUtc) GetToken(UserDto users)
    {
        var handler = new JwtSecurityTokenHandler();

        var privateKey = Encoding.UTF8.GetBytes(JwtConfiguration.PrivateKey);

        var credentials = new SigningCredentials(new SymmetricSecurityKey(privateKey), SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddHours(12);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            SigningCredentials = credentials,
            Expires = expires,
            Subject = GenerateClaims(users)
        };

        var token = handler.CreateToken(tokenDescriptor);
        return (handler.WriteToken(token), expires);
    }

    private ClaimsIdentity GenerateClaims(UserDto users)
    {
        var ci = new ClaimsIdentity("token");
        ci.AddClaim(new Claim(ClaimTypes.NameIdentifier, users.Id.ToString()));
        ci.AddClaim(new Claim(ClaimTypes.Email, users.Email));

        return ci;
    }


    public async Task<UsersResponseTokenDto> CreateUserEmployeeAsync(RegisterEmployeeDto dataEmployeeEmployeeDto)
    {
        var existingEmail = await appDbContext.Users
            .AnyAsync(e => e.Email == dataEmployeeEmployeeDto.Email);

        var existingUserCompany =
            await appDbContext.Users.FirstOrDefaultAsync(e => e.Id == dataEmployeeEmployeeDto.CompanyId);
        var existingCompany = await appDbContext.Companies.FirstOrDefaultAsync(e => e.Id == dataEmployeeEmployeeDto.CompanyId);
        
        if (existingEmail)
            throw new ValidationException("This Email can't be used");

        if (existingUserCompany == null)
        {
            throw new ValidationException("User not Found");
        }

        if (!existingUserCompany.Roles.Contains("admin"))
        {
            throw new ValidationException("You don't have permission for create user");
        }

        await using var transaction = await appDbContext.Database.BeginTransactionAsync();

        try
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dataEmployeeEmployeeDto.Password);

            var createdUser = new Users
            {
                Email = dataEmployeeEmployeeDto.Email,
                Password = passwordHash,
                Roles = dataEmployeeEmployeeDto.Roles,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };


            await appDbContext.Users.AddAsync(createdUser);
            await appDbContext.SaveChangesAsync();

            if (string.IsNullOrWhiteSpace(dataEmployeeEmployeeDto.Name))
                throw new ValidationException("Name is required for employee");

            if (string.IsNullOrWhiteSpace(dataEmployeeEmployeeDto.Job))
                throw new ValidationException("Job is required for employee");

            if (!dataEmployeeEmployeeDto.Matriculation.HasValue)
                throw new ValidationException("Matriculation is required for employee");

            var employee = new Employee()
            {
                Name = dataEmployeeEmployeeDto.Name,
                Job = dataEmployeeEmployeeDto.Job,
                Matriculation = dataEmployeeEmployeeDto.Matriculation.Value,
                UserId = createdUser.Id,
                
                CompanyId = existingUserCompany.Id,
                CompanyName = existingCompany.Name,
            };

            await appDbContext.Employees.AddAsync(employee);


            await appDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new UsersResponseTokenDto()
            {
                StatusCode = 201,
                Message = "User created successfully",
                Authentication = true,
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<UsersResponseTokenDto> CreatUserCompaniesAsync(RegisterComapiesDto dataEmployeeDto)
    {
        var existingEmail = await appDbContext.Users
            .AnyAsync(e => e.Email == dataEmployeeDto.Email);

        if (existingEmail)
            throw new ValidationException("This Email can't be used");

        await using var transaction = await appDbContext.Database.BeginTransactionAsync();

        try
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dataEmployeeDto.Password);

            var createdUser = new Users
            {
                Email = dataEmployeeDto.Email,
                Password = passwordHash,
                Roles = dataEmployeeDto.Roles,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await appDbContext.Users.AddAsync(createdUser);
            await appDbContext.SaveChangesAsync();


            var company = new Companies
            {
                Name = dataEmployeeDto.CompanyName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = createdUser.Id
            };


            await appDbContext.Companies.AddAsync(company);
            await appDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new UsersResponseTokenDto()
            {
                StatusCode = 201,
                Message = "User created successfully",
                Authentication = true,
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<Users>> GetUserAsync()
    {
        var allUsers = await appDbContext.Users.ToListAsync();
        return allUsers;
    }


    public async Task<UsersResponseDto> UpdateUserAsync(LoginDto dataDto, int id)
    {
        var updatedUser = await appDbContext.Users.FindAsync(id);
        if (updatedUser == null)
        {
            throw new NotFoundException(404, "User not found!");
        }

        if (dataDto == null)
        {
            throw new ValidationException("Invalid data!");
        }


        updatedUser.Email = dataDto.Email;
        updatedUser.Password = dataDto.Password;
        updatedUser.UpdatedAt = DateTime.UtcNow;


        await appDbContext.SaveChangesAsync();

        var response = new UsersResponseDto()
        {
            Email = updatedUser.Email,
            UpdatedAt = updatedUser.UpdatedAt,
        };

        return response;
    }

    public async Task<UsersMessageDto> DeleteUserAsync(int id)
    {
        var deleted = await appDbContext.Users.FindAsync(id);
        if (deleted == null)
        {
            throw new NotFoundException(404, "User not found!");
        }

        appDbContext.Remove(deleted);
        await appDbContext.SaveChangesAsync();


        var response = new UsersMessageDto()
        {
            StatusCode = 200,
            Message = "User Deleted successfully",
        };

        return response;
    }
}