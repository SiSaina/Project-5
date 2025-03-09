using ExamProjectOne.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamProjectOne.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public int ScheduleId { get; set; }
        [Required]
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        [Required]
        [ForeignKey("ScheduleId")]
        public Schedule Schedule { get; set; }
    }
}
