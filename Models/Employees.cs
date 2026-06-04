using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TimeRecord.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string Job { get; set; }

        [Required] public int Matriculation { get; set; }

        [Required] public int UserId { get; set; }

        [ForeignKey((nameof(UserId)))]
        [JsonIgnore]
        public Users Users { get; set; }

        public int CompanyId { get; set; }

        [ForeignKey((nameof(CompanyId)))]
        [JsonIgnore]
        public Companies Companies { get; set; }
    }
}