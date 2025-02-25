using ExamProjectOne.Data;
using ExamProjectOne.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel.DataAnnotations;
using System.Data;
using ExamProjectOne.Data;
using ExamProjectOne.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExamProjectOne.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EmployeeController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public EmployeeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<EmployeeController> logger, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _signInManager = signInManager;
        }

        public IActionResult Employee()
        {
            ViewBag.Supervisors = _context.Supervisors.Include(s => s.User).ToList();
            ViewBag.Coaches = _context.Coaches.Include(c => c.User).ToList();
            ViewBag.AspNetUsers = _context.Users.ToList();

            return View();
        }

        [HttpGet]
        public IActionResult AddEmployee()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddEmployee([Bind("Specialization, WorkingHour")] EmployeeModel model)
        {
            if (ModelState.IsValid)
            {
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

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (model.Role == "Coach")
                    {
                        string specialization = Request.Form["Specialization"];
                        TimeOnly workingHour = TimeOnly.Parse(Request.Form["WorkingHour"]);

                        var coach = new Coach
                        {
                            UserId = user.Id,
                            Specialization = specialization,
                            WorkingHour = workingHour,
                            Status = model.Status
                        };
                        _context.Coaches.Add(coach);
                        await _userManager.AddToRoleAsync(user, model.Status == "Regular" ? "Coach" : "Senior Coach");

                    }
                    else if (model.Role == "Supervisor")
                    {
                        var supervisor = new Supervisor
                        {
                            UserId = user.Id,
                            Status = model.Status
                        };
                        _context.Supervisors.Add(supervisor);
                        await _userManager.AddToRoleAsync(user, model.Status == "Regular" ? "Supervisor" : "Senior supervisor");
                    }

                    return RedirectToAction("Employee");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }


    }
}
