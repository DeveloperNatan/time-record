// using TimeRecord.Data;
// using TimeRecord.Models;

// namespace TimeRecord.Validation
// {
//     public static class EmployeeValidation
//     {
//         private readonly AppDbContext _appdbcontext;

//         public SearchUser(AppDbContext appdbcontext)
//         {
//             _appdbcontext = appdbcontext;
//         }

//         public async Task<Employee> FindOne(int id)
//         {
//             var User = await _appdbcontext.Employees.FindAsync(id);
//             if (User == null)
//             {
//                 throw new KeyNotFoundException("Usuario não encontrado!");
//             }
//             return User;
//         }
//     }
// }
