using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamProjectOne.Models
{
    public class Schedule
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CoachId { get; set; }
        [Required]
        public int GymHallId { get; set; }
        [Required]
        public TimeOnly StartTime { get; set; }
        [Required]
        public TimeOnly EndTime { get; set; }
        [Required]
        public DateTime Date { get; set; }
        //For group or individual
        [Required]
        public bool GroupSession { get; set; }
        [ForeignKey("CoachId")]
        public Coach Coach { get; set; }
        [ForeignKey("GymHallId")]
        public GymHall GymHall { get; set; }
    }
}
