using ExamProjectOne.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamProjectOne.Models
{
    public class Coach
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Status { get; set; } = string.Empty;
        [Required]
        public string Specialization { get; set; } = string.Empty;
        [Required]
        public TimeOnly WorkingHour { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
