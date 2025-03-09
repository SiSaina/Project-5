using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamProjectOne.Models
{
    public class GroupTraining
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int? ScheduleId { get; set; }
        [Required]
        public int Capacity { get; set; }
        [ForeignKey("ScheduleId")]
        public Schedule? Schedule { get; set; }
        [Required]
        public List<GroupTrainingCustomer> GroupTrainingCustomers { get; set; } = [];
    }
}
