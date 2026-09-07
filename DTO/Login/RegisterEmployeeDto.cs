namespace TimeRecord.DTO.Login;

public enum UserProfileType
{
    Employee = 1,
    Companies = 2
}

public class RegisterEmployeeDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string[] Roles { get; set; } = null!;
  

    // Employee
    public string? Name { get; set; }
    public string? Job { get; set; }
    public int? Matriculation { get; set; }
    public int? CompanyId { get; set; }

 
}

