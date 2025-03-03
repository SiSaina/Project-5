using ExamProjectOne.Data;
using ExamProjectOne.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Data;

namespace ExamProjectOne.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public EmployeeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Read()
        {
            var person = await _context.Users
                .Include(u => u.Supervisor)
                .Include(u => u.Coach)
                .Include(u => u.Customer)
                .ToListAsync();

            var people = person.Select(user => new UserModel
            {
                IdStr = user.Id.Substring(0, 3),
                Username = user.UserName ?? "",
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                BirthDate = user.DateOfBirth.Date,
                RoleStats = GetRoleNStatus(user),
                ShiftTime = user.Supervisor?.ShiftTime ?? user.Coach?.ShiftTime ?? "",
                WorkDay = user.Supervisor?.WorkDay ?? user.Coach?.WorkDay ?? "",
                Skill = user.Coach?.Skill ?? ""
            }).ToList();
            return View(people);
        }



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

            if (!result.Succeeded) return View(model);

            //Create employee
            var person = CreateEmployee(model, user.Id);
            if (person is Coach coach) _context.Coaches.Add(coach);
            else if (person is Supervisor supervisor) _context.Supervisors.Add(supervisor);
            else if (person is Customer customer) _context.Customers.Add(customer);

            //Role
            string status = model.Status == "" ? model.Role : $"Senior {model.Role}";
            await _userManager.AddToRoleAsync(user, status);

            await _context.SaveChangesAsync();
            return RedirectToAction("Read");
        }

        public IActionResult Update(string id)
        {
            var person = _context.Users
                .Include(u => u.Supervisor)
                .Include(u => u.Coach)
                .Include(u => u.Customer)
                .FirstOrDefault(u => u.Id == id);

            return View(person);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UserModel model)
        {
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var coach = await _context.Coaches.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == model.Id);
            string? skill = Request.Form["Skill"];
            if (coach != null)
            {
                coach.Status = model.Status;
                coach.ShiftTime = model.ShiftTime;
                coach.WorkDay = model.WorkDay;
                coach.Skill = skill ?? "";
                UpdateUser(coach.User, model);
                await UpdateRole(coach.User, model.Role, model.Status);
            }
            else
            {
                var supervisor = await _context.Supervisors.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == model.Id);

                supervisor.Status = model.Status;
                supervisor.ShiftTime = model.ShiftTime;
                supervisor.WorkDay = model.WorkDay;
                UpdateUser(supervisor.User, model);
                await UpdateRole(supervisor.User, model.Role, model.Status);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Read");
        }

        [HttpGet]
        public IActionResult Delete(int id, string role)
        {
            var em = role?.ToLower() == "coach"
                    ? _context.Coaches.Include(c => c.User).FirstOrDefault(e => e.Id == id) as dynamic
                    : _context.Supervisors.Include(s => s.User).FirstOrDefault(e => e.Id == id);

            var model = new UserModel
            {
/*                Id = em.Id,
                Username = em.User?.UserName ?? "",
                FirstName = em.User?.FirstName ?? "",
                LastName = em.User?.LastName ?? "",
                Email = em.User?.Email ?? "",
                PhoneNumber = em.User?.PhoneNumber ?? "",
                BirthDate = em.User?.DateOfBirth,
                Gender = em.User?.Gender ?? "",
                Status = em.Status,
                Role = role ?? "Regular",
                ShiftTime = em.ShiftTime,
                WorkDay = em.WorkDay,*/
            };

            if (role?.ToLower() == "Coach")
            {
                ViewBag.Specialize = em.Specialize;
            }

            return View(model);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, string role)
        {
            var em = role?.ToLower() == "coach"
                    ? _context.Coaches.Include(c => c.User).FirstOrDefault(e => e.Id == id) as dynamic
                    : _context.Supervisors.Include(s => s.User).FirstOrDefault(e => e.Id == id);

            if (em == null) return NotFound();

            if (role.ToLower() == "coach")
            {
                _context.Coaches.Remove(em);
            }
            else if (role.ToLower() == "supervisor")
            {
                _context.Supervisors.Remove(em);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Read");
        }

        //Create coach or supervisor
        private object CreateEmployee(UserModel model, string userId)
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
        private async Task UpdateRole(ApplicationUser user, string role, string status)
        {
            var Role = await _userManager.GetRolesAsync(user);
            var targetRole = status == "Regular" ? role : $"Senior {role}";

            if (!Role.Contains(targetRole))
            {
                foreach (var existingRole in Role)
                {
                    await _userManager.RemoveFromRoleAsync(user, existingRole);
                }
                await _userManager.AddToRoleAsync(user, targetRole);
            }
        }


        private List<string> GetRoleNStatus(ApplicationUser user)
        {
            var result = new List<string>();

            if (user.Supervisor != null)
            {
                string status = user.Supervisor.Status == "Senior" ? "Senior" : "";
                result.Add($"{status} Supervisor");
            }

            if (user.Coach != null)
            {
                string status = user.Coach.Status == "Senior" ? "Senior" : "";
                result.Add($"{status} Coach");
            }

            if (user.Customer != null)
            {
                result.Add("Customer");
            }

            return result;
        }
    }
}
