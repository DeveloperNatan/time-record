namespace TimeRecord.DTO.Login;

public enum UserProfileType
{
    Employee = 1,
    Companies = 2
}

public class RegisterEmployeeDto
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;

    public string[] Roles { get; set; } = default!;
  

    // Employee
    public string? Name { get; set; }
    public string? Job { get; set; }
    public int? Matriculation { get; set; }
    public int? CompanyId { get; set; }

 
}

