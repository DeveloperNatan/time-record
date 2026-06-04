using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeRecord.Models
{
    public class Token
    {
       public string AcecessToken { get; set; }
       
       public string TokenType { get; set; }
       
       public int  ExpiresIn { get; set; }
    }
}