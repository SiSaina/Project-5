using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace ExamProjectOne.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public DateTime DateOfBirth { get; set; } = new DateTime(1111, 1, 1);

        [Required]
        public string Gender { get; set; } = "Unknown";
        public string? CoachId { get; set; }
        [ForeignKey("CoachId")]
        public ApplicationUser? Coach { get; set; }
        public string? SupervisorId { get; set; }
        [ForeignKey("SupervisorId")]
        public ApplicationUser? Supervisor { get; set; }
        public string? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public ApplicationUser? Customer { get; set; }
    }
}
