using ExamProjectOne.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamProjectOne.Models
{
    public class UserPermission
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        [Required]
        public int PermissionId { get; set; }
        [Required]
        [ForeignKey("PermissionId")]
        public Permission Permission { get; set; } = null!;
        [Required]
        public string Entity { get; set; } = string.Empty;
    }
}
