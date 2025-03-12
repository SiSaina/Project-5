using ExamProjectOne.Data;
using ExamProjectOne.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExamProjectOne.Controllers
{
    [Authorize(Roles = "Admin, Senior supervisor")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationUser? _person;
        private List<ApplicationUser> _people;
        public EmployeeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        //Query user from database
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.TryGetValue("id", out var idObj) && idObj is string id)
            {
                _person = await _context.Users
                    .Include(u => u.Supervisor)
                    .Include(u => u.Coach)
                    .Include(u => u.Customer)
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
            _people ??= await _context.Users
                    .Where(u => u.Coach != null || u.Supervisor != null)
                    .Include(u => u.Supervisor)
                    .Include(u => u.Coach)
                    .Include(u => u.Customer)
                    .ToListAsync();
            await next();
        }
        //Read
        public IActionResult Read()
        {
            var people = _people.Select(user => new UserModel
            {
                IdStr = user.Id,
                Username = user.UserName ?? "",
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                BirthDate = user.DateOfBirth,
                RoleStats = GetRoleNStatus(user),
                ShiftTimeList = GetShiftTimeList(user),
                WorkDayList = GetWorkDayList(user),
                Skill = user.Coach?.Skill ?? ""
            }).ToList();
            return View(people);
        }
        //Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.BirthDate,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return View("Error");

            //Create employee
            var person = CreateEmployee(model, user.Id);
            if (person is Coach coach) _context.Coaches.Add(coach);
            else if (person is Supervisor supervisor) _context.Supervisors.Add(supervisor);
            else if (person is Customer customer) _context.Customers.Add(customer);

            //Role
            string RoleStatus = model.Status == "Regular" ? model.Role : $"Senior {model.Role}";
            await _userManager.AddToRoleAsync(user, RoleStatus);

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Created successfully";
            return RedirectToAction("Read");
        }

        //Update
        [HttpGet]
        public IActionResult Update(string id)
        {
            var user = new UserModel
            {
                IdStr = _person.Id,
                Username = _person.UserName ?? "",
                FirstName = _person.FirstName,
                LastName = _person.LastName,
                Gender = _person.Gender,
                Email = _person.Email ?? "",
                PhoneNumber = _person.PhoneNumber ?? "",
                BirthDate = _person.DateOfBirth,
                RoleList = GetRoleList(_person),
                
                StatusCoach = _person.Coach?.Status,
                StatusSupervisor = _person.Supervisor?.Status,
                ShiftTimeCoach = _person.Coach?.ShiftTime,
                ShiftTimeSupervisor = _person.Supervisor?.ShiftTime,
                WorkDayCoach = _person.Coach?.WorkDay,
                WorkDaySupervisor = _person.Supervisor?.WorkDay,

                Skill = _person.Coach?.Skill ?? ""
            };

            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UserModel model, string id, string action)
        {
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Id");
            if (!ModelState.IsValid) return View(model);

            if (action == "Coach" && _person?.Coach != null)
            {
                _person.Coach.Status = model.Status ?? _person.Coach.Status;
                _person.Coach.ShiftTime = model.ShiftTime ?? _person.Coach.ShiftTime;
                _person.Coach.WorkDay = model.WorkDay ?? _person.Coach.WorkDay;
                _person.Coach.Skill = model.Skill ?? _person.Coach.Skill;
                await AddRole(_person.Coach.User, "Coach", model.Status);
            }
            if (action == "Supervisor" && _person?.Supervisor != null)
            {
                _person.Supervisor.Status = model.Status ?? _person.Supervisor.Status;
                _person.Supervisor.ShiftTime = model.ShiftTime ?? _person.Supervisor.ShiftTime;
                _person.Supervisor.WorkDay = model.WorkDay ?? _person.Supervisor.WorkDay;
                await AddRole(_person.Supervisor.User, "Supervisor", model.Status);
            }
            UpdateUser(_person, model);

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Updated successfully";
            return RedirectToAction("Read");
        }

        //Delete
        [HttpGet]
        public IActionResult Delete(string id)
        {
            var user = new UserModel
            {
                IdStr = _person.Id,
                Username = _person.UserName ?? "",
                FirstName = _person.FirstName,
                LastName = _person.LastName,
                Gender = _person.Gender,
                Email = _person.Email ?? "",
                PhoneNumber = _person.PhoneNumber ?? "",
                RoleStats = GetRoleNStatus(_person)
            };

            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {

            var roles = await _userManager.GetRolesAsync(_person);
            foreach (var existingRole in roles)
            {
                await _userManager.RemoveFromRoleAsync(_person, existingRole);
            }
            if (_person.Coach != null) _context.Coaches.Remove(_person.Coach);
            if (_person.Supervisor != null) _context.Supervisors.Remove(_person.Supervisor);
            if (_person.Customer != null) _context.Customers.Remove(_person.Customer);

            _context.Users.Remove(_person);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Deleted successfully";
            return RedirectToAction("Read");
        }

        //Promote
        [HttpGet]
        public IActionResult Promote(string id)
        {
            var user = new UserModel
            {
                IdStr = _person.Id,
                Username = _person.UserName ?? "",
                FirstName = _person.FirstName,
                LastName = _person.LastName,
                Gender = _person.Gender,
                IsCoach = _person.Coach != null,
                IsSupervisor = _person.Supervisor != null,
                IsCustomer = _person.Customer != null,

                StatusCoach = _person.Coach?.Status ?? "",
                StatusSupervisor = _person.Supervisor?.Status ?? ""
            };

            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> PromoteConfirmed(string id, UserModel model)
        {
            if (model.IsCoach.GetValueOrDefault())
            {
                if (_person.Coach == null)
                {
                    _person.Coach = new Coach { UserId = id, Status = model.StatusCoach ?? "" };
                    string role = model.StatusCoach == "Regular" ? "Coach" : $"Senior Coach";
                    await AddRole(_person, "coach", model.StatusCoach);
                    await _userManager.AddToRoleAsync(_person, role);
                }
                else
                {
                    _person.Coach.Status = model.StatusCoach ?? "Regular";
                    await AddRole(_person, "coach", model.StatusCoach);
                }
            }
            else 
            {
                await RemoveRole(_person, "coach", model.StatusCoach);
                _person.Coach = null;
            }

            if (model.IsSupervisor.GetValueOrDefault())
                if (_person.Supervisor == null)
                {
                    _person.Supervisor = new Supervisor { UserId = id, Status = model.StatusSupervisor ?? "Regular" };
                    string role = model.StatusSupervisor == "Regular" ? "Supervisor" : $"Senior Supervisor";
                    await AddRole(_person, "supervisor", model.StatusSupervisor);
                    await _userManager.AddToRoleAsync(_person, role);
                }
                else
                {
                    _person.Supervisor.Status = model.StatusSupervisor ?? "Regular";
                    await AddRole(_person, "supervisor", model.StatusSupervisor);
                }
            else
            {
                await RemoveRole(_person, "supervisor", model.StatusSupervisor);
                _person.Supervisor = null;
            }

            if (model.IsCustomer.GetValueOrDefault())
            {
                if (_person.Customer == null)
                {
                    _person.Customer = new Customer { UserId = id };
                    await _userManager.AddToRoleAsync(_person, "Customer");
                }
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(_person, "Customer");
                _person.Customer = null;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Promoted successfully";
            return RedirectToAction("Read");
        }

        //Create coach or supervisor
        private static object CreateEmployee(UserModel model, string userId)
        {
            return model.Role switch
            {
                "Coach" => new Coach
                {
                    UserId = userId,
                    Status = model.Status ?? "",
                    Skill = model.Skill ?? "Unknown",
                    ShiftTime = model.ShiftTime ?? "",
                    WorkDay = model.WorkDay ?? "",
                },

                "Supervisor" => new Supervisor
                {
                    UserId = userId,
                    Status = model.Status ?? "",
                    ShiftTime = model.ShiftTime ?? "",
                    WorkDay = model.WorkDay ?? ""
                },
                "Customer" => new Customer
                {
                    UserId = userId
                },

                _ => throw new ArgumentException("Invalid role")
            };
        }
        //Update coach or supervisor
        private static void UpdateUser(ApplicationUser user, UserModel model)
        {
            user.UserName = model.Username;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Gender = model.Gender;
        }
        private async Task AddRole(ApplicationUser user, string role, string status)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var targetRole = status == "Regular" ? role : $"Senior {role}";

            var roleToRemove = roles.FirstOrDefault(r =>
                r.Equals(role, StringComparison.OrdinalIgnoreCase) ||
                r.Equals($"Senior {role}", StringComparison.OrdinalIgnoreCase));
            if (roleToRemove != null && roleToRemove != targetRole)
            {
                await _userManager.RemoveFromRoleAsync(user, roleToRemove);
            }

            if (!roles.Contains(targetRole))
            {
                await _userManager.AddToRoleAsync(user, targetRole);
            }
        }
        private async Task RemoveRole(ApplicationUser user, string role, string status)
        {
            var Role = await _userManager.GetRolesAsync(user);
            var targetRole = status == "Regular" ? role : $"Senior {role}";

            if (Role.Contains(targetRole))
            {
                await _userManager.RemoveFromRoleAsync(user, targetRole);
            }
        }

        //Get small properties
        private static List<string> GetRoleNStatus(ApplicationUser user)
        {
            List<string> result = [];
            if (user.Coach != null)
            {
                string status = user.Coach.Status == "Senior" ? "Senior" : "";
                result.Add($"{status} Coach");
            }
            if (user.Supervisor != null)
            {
                string status = user.Supervisor.Status == "Senior" ? "Senior" : "";
                result.Add($"{status} Supervisor");
            }
            if (user.Customer != null)
            {
                result.Add("Customer");
            }
            return result;
        }
        private static List<string> GetRoleList(ApplicationUser user)
        {
            List<string> result = [];
            if (user.Coach != null) result.Add("Coach");
            if (user.Supervisor != null) result.Add("Supervisor");
            if (user.Customer != null) result.Add("Customer");
            return result;
        }
        private static List<string> GetShiftTimeList(ApplicationUser user)
        {
            List<string> result = [];
            if (user.Coach != null) result.Add(user.Coach.ShiftTime);
            if (user.Supervisor != null) result.Add(user.Supervisor.ShiftTime);
            return result;
        }
        private static List<string> GetWorkDayList(ApplicationUser user)
        {
            List<string> result = [];
            if (user.Coach != null) result.Add(GetWorkDayType(user.Coach.WorkDay));
            if (user.Supervisor != null) result.Add(GetWorkDayType(user.Supervisor.WorkDay));
            return result;
        }
        private static string GetWorkDayType(string workDay)
        {
            if (workDay == "Monday - Friday")
            {
                return "Weekday";
            }
            else if (workDay == "Saturday - Sunday")
            {
                return "Weekend";
            }
            return "";
        }
    }
}
