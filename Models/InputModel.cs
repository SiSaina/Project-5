using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamProjectOne.Models
{
    [NotMapped]
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
    [NotMapped]
    public class RegisterModel : LogInModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateOnly BirthDate { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; } = string.Empty;
    }
    [NotMapped]
    public class UserModel : RegisterModel
    {
        public int Id { get; set; }
        public string? IdStr { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;
        public string? Status { get; set; } = string.Empty;
        public string? ShiftTime { get; set; }
        public string? WorkDay { get; set; }
        public string? Skill { get; set; } = string.Empty;

        public List<string>? RoleStats { get; set; } = [];
        public List<string>? RoleList { get; set; } = [];
        public List<string>? ShiftTimeList { get; set; } = [];
        public List<string>? WorkDayList { get; set; } = [];

        public string? StatusCoach { get; set; } = string.Empty;
        public string? StatusSupervisor { get; set; } = string.Empty;
        public string? ShiftTimeCoach { get; set; } = string.Empty;
        public string? ShiftTimeSupervisor{ get; set; } = string.Empty;
        public string? WorkDayCoach { get; set; } = string.Empty;
        public string? WorkDaySupervisor { get; set; } = string.Empty;

        public bool? IsCoach { get; set; } = false;
        public bool? IsSupervisor { get; set; } = false;
        public bool? IsCustomer { get; set; } = false;
    }
    [NotMapped]
    public class ScheduleModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public TimeOnly StartTime { get; set; }
        [Required]
        public TimeOnly EndTime { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public bool GroupSession { get; set; }
        [Required]
        public int Capacity { get; set; }

        public List<Customer>? Customers { get; set; } = [];
        public List<Coach>? Coaches { get; set; } = [];
        public List<GymHall>? GymHalls { get; set; } = [];

        public Customer? CustomerOne { get; set; }
        public Coach? CoachOne { get; set; }
        public GymHall? GymHallOne { get; set; }

        public int CustomerId { get; set; }
        public int CoachId { get; set; }
        public int GymHallId { get; set; }

        public List<int?> SelectCustomer { get; set; } = [];

        public string CoachName { get; set; } = string.Empty;
        public string GymHallName { get; set; } = string.Empty;

        public string Mode { get; set; } = string.Empty;
    }

}
