using System.ComponentModel.DataAnnotations;

namespace ExamProjectOne.Models
{
    public class GymHall
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Capacity { get; set; }
    }
}
