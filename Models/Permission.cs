using System.ComponentModel.DataAnnotations;

namespace ExamProjectOne.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public ICollection<UserPermission> UserPermissions { get; set; } = [];

    }
}
