using System.ComponentModel.DataAnnotations;
using TimeRecord.DTO.Company;

namespace TimeRecord.Validation
{
    public class BusinessValidator
    {
        public static void Validate(CompanyCreateDto company)
        {
            if (string.IsNullOrWhiteSpace(company.Name))
            {
                throw new ValidationException("Enter a name valid");
            }
            
        }
    }
}