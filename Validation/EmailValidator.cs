// using System.Net.Mail;
// using TimeRecord.DTO.Employee;
//
// namespace TimeRecord.Validation
// {
//     public class EmailValidator
//     {
//         public static bool IsValidEmail(EmployeeCreateAndUpdateDto employee)
//         {
//             try
//             {
//                 var email = employee.Email;
//                 var m = new MailAddress(email);
//
//                 if(m.Address != email)
//                 {
//                     return false;
//                 }
//                 var domain = m.Host;
//                 if(!domain.Contains('.')){ return false;}
//                 if(domain.StartsWith(".") || domain.EndsWith('.')) { return false; }
//
//                 return true;
//             }
//             catch
//             {
//                 return false;
//             }
//         }
//     }
// }
