using System.ComponentModel.DataAnnotations;

namespace ExamProjectOne.Models
{
    public class LogInModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
    }
    public class RegisterModel : LogInModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty; // Changed from int to string

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
    public class EmployeeModel : RegisterModel
    {
        [Required]
        public string Role { get; set; } = string.Empty;
        [Required]
        public string Status { get; set; } = string.Empty;
    }

}
