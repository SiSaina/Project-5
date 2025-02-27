using ExamProjectOne.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamProjectOne.Models
{
    public class Supervisor
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? UserId { get; set; }
        [Required]
        public string? Status { get; set; }
        [Required]
        public string ShiftTime { get; set; }
        [Required]
        public string WorkDay { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
