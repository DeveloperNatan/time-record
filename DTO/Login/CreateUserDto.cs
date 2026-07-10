namespace TimeRecord.DTO.Login;

public enum UserProfileType
{
    Employee = 1,
    Company = 2
}

public class CreateUserDto
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public UserProfileType ProfileType { get; set; }

    // Employee
    public string? Name { get; set; }
    public string? Job { get; set; }
    public int? Matriculation { get; set; }
    public int? CompanyId { get; set; }

    // Company
    public string? CompanyName { get; set; }
}