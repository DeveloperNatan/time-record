using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TimeRecord.Data;
using TimeRecord.DTO.Auth;
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
            return BCrypt.Net.BCrypt.Verify(passwordEntered, userDb.PasswordHash);
        }

        if (!VerifyPassword(password))
        {
            throw new UnauthorizedAccessException("Password incorrect!");
        }

        var user = new Users
        {
            Id = userDb.Id,
            Email = userDb.Email,
            PasswordHash = userDb.PasswordHash,
            Roles = new[] { "developer" }
        };

        var (token, expiresUtc) = GetToken(user);

        return new Token()
        {
            AcecessToken = token,
            TokenType = "Bearer",
            ExpiresIn = (int)(expiresUtc - DateTime.UtcNow).TotalSeconds,
        };
    }

    private (string Token, DateTime ExpiresUtc) GetToken(Users users)
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

    private ClaimsIdentity GenerateClaims(Users users)
    {
        var ci = new ClaimsIdentity("token");
        ci.AddClaim(new Claim(ClaimTypes.NameIdentifier, users.Id.ToString()));
        ci.AddClaim(new Claim(ClaimTypes.Email, users.Email));

        return ci;
    }


    public async Task<UsersResponseTokenDTO> CreateUserEmployeeAsync(RegisterEmployeeDto dataEmployeeEmployeeDto)
    {
        var existingEmail = await appDbContext.Users
            .AnyAsync(e => e.Email == dataEmployeeEmployeeDto.Email);

        if (existingEmail)
            throw new ValidationException("This Email can't be used");

        await using var transaction = await appDbContext.Database.BeginTransactionAsync();

        try
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dataEmployeeEmployeeDto.Password);

            var createdUser = new Users
            {
                Email = dataEmployeeEmployeeDto.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await appDbContext.Users.AddAsync(createdUser);
            Console.Write(createdUser.Id);
            await appDbContext.SaveChangesAsync();

            if (dataEmployeeEmployeeDto.ProfileType == UserProfileType.Employee)
            {
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
                    CompanyId = 2,
                };

                await appDbContext.Employees.AddAsync(employee);
            }
            else if (dataEmployeeEmployeeDto.ProfileType == UserProfileType.Companies)
            {
                if (string.IsNullOrWhiteSpace(dataEmployeeEmployeeDto.CompanyName))
                    throw new ValidationException("CompanyName is required for companyv");

                var company = new Companies
                {
                    Name = dataEmployeeEmployeeDto.CompanyName,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = createdUser.Id
                };

                await appDbContext.Companies.AddAsync(company);
            }
            else
            {
                throw new ValidationException("Invalid profile type");
            }

            await appDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new UsersResponseTokenDTO()
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

    public async Task<UsersResponseTokenDTO> CreatUserCompaniesAsync(RegisterComapiesDto dataEmployeeDto)
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
                PasswordHash = passwordHash,
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

            return new UsersResponseTokenDTO()
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


    public async Task<UsersResponseDTO> UpdateUserAsync(LoginDto dataDto, int id)
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
        updatedUser.PasswordHash = dataDto.PasswordHash;
        updatedUser.UpdatedAt = DateTime.UtcNow;


        await appDbContext.SaveChangesAsync();

        var response = new UsersResponseDTO()
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