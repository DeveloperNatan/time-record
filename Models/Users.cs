using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeRecord.Models
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MinLength(5)]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string PasswordHash { get; set; }

        [MinLength(3)] [MaxLength(30)] public string[] Roles { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}