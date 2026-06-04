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

public class AuthService(AppDbContext appDbContext)
{
    public async Task<Token> GenerateToken(string email, string password)
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


    public async Task<AuthResponseTokenDTO> CreateUserAsync(LoginDto dataDto)
    {
        var existingEmail = await appDbContext.Users.AnyAsync(e => e.Email == dataDto.Email);

        if (existingEmail)
        {
            throw new ValidationException("This Email can't be used");
        }

        dataDto.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dataDto.PasswordHash);

        var createdEmail = new Users()
        {
            Email = dataDto.Email,
            PasswordHash = dataDto.PasswordHash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await appDbContext.Users.AddAsync(createdEmail);
        await appDbContext.SaveChangesAsync();


        var response = new AuthResponseTokenDTO()
        {
            StatusCode = 201,
            Message = "User created successfully",
            Authentication = true,
        };

        return response;
    }

    public async Task<IEnumerable<Users>> GetUserAsync()
    {
        var allUsers = await appDbContext.Users.ToListAsync();
        return allUsers;
    }


    public async Task<AuthResponseDTO> UpdateUserAsync(LoginDto dataDto, int id)
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

        var response = new AuthResponseDTO()
        {
            Email = updatedUser.Email,
            UpdatedAt = updatedUser.UpdatedAt,
        };

        return response;
    }

    public async Task<AuthMessageDto> DeleteUserAsync(int id)
    {
        var deleted = await appDbContext.Users.FindAsync(id);
        if (deleted == null)
        {
            throw new NotFoundException(404, "User not found!");
        }

        appDbContext.Remove(deleted);
        await appDbContext.SaveChangesAsync();


        var response = new AuthMessageDto()
        {
            StatusCode = 200,
            Message = "User Deleted successfully",
        };

        return response;
    }
}