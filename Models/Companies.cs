using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TimeRecord.Models
{
    public class Companies
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required] public bool IsActive { get; set; } = true;

        [Required] public int UserId { get; set; }

        [ForeignKey((nameof(UserId)))]
        [JsonIgnore]
        public Users Users { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}