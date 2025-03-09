using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamProjectOne.Models
{
    public class GroupTrainingCustomer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int GroupTrainingId { get; set; }
        [ForeignKey("GroupTrainingId")]
        public GroupTraining GroupTraining { get; set; }
        [Required]
        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
    }
}
