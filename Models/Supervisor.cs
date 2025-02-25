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
        public string UserId { get; set; }
        [Required]
        public string Status { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
