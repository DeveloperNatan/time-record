using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace TimeRecord.Models
{
    public class TimeRecords
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required] public int EmployeeId { get; set; }
        [ForeignKey((nameof(EmployeeId)))] public Employee Employee { get; set; }
        [Required] public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        [JsonIgnore]
        public Companies Companies { get; set; }
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    }
}