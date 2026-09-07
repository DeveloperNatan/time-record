namespace TimeRecord.DTO.Login;

public class RegisterComapiesDto
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string CompanyName { get; set; } = default!;
    public string[] Roles { get; set; }
}