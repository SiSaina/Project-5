﻿using ExamProjectOne.Data;
using ExamProjectOne.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Data;

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

        public async Task<IActionResult> Employee()
        {
            var supervisors = await _context.Supervisors.Include(s => s.User).ToListAsync();
            var coaches = await _context.Coaches.Include(c => c.User).ToListAsync();

            return View(new { Supervisors = supervisors, Coaches = coaches });
        }

        [HttpGet]
        public IActionResult AddEmployee()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddEmployee(EmployeeModel model)
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

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(model);
            }

            //Create employee
            var employee = CreateEmployee(model, user.Id);
            if (employee is Coach coach)
                _context.Coaches.Add(coach);
            else if (employee is Supervisor supervisor)
                _context.Supervisors.Add(supervisor);

            //Role
            string role = model.Status == "Regular" ? model.Role : $"Senior {model.Role}";
            await _userManager.AddToRoleAsync(user, role);

            await _context.SaveChangesAsync();
            return RedirectToAction("Employee");
        }

        public IActionResult EditEmployee(int id, string role)
        {
            var em = role?.ToLower() == "coach"
                ? _context.Coaches.Include(c => c.User).FirstOrDefault(e => e.Id == id) as dynamic
                : _context.Supervisors.Include(s => s.User).FirstOrDefault(e => e.Id == id);

            if (em == null) return NotFound(); 

            var model = new EmployeeModel
            {
                Id = em.Id,
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
                WorkDay = em.WorkDay,
                Specialize = role?.ToLower() == "coach" ? em.Specialize : null,
            };


            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditEmployee(EmployeeModel model)
        {
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var coach = await _context.Coaches.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == model.Id);
            string? specizalize = Request.Form["Specialize"];
            if (coach != null)
            {
                coach.Status = model.Status;
                coach.ShiftTime = model.ShiftTime;
                coach.WorkDay = model.WorkDay;
                coach.Specialize = specizalize ?? "";
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
            return RedirectToAction("Employee");
        }

        [HttpGet]
        public IActionResult DeleteEmployee(int id, string role)
        {
            var em = role?.ToLower() == "coach"
                    ? _context.Coaches.Include(c => c.User).FirstOrDefault(e => e.Id == id) as dynamic
                    : _context.Supervisors.Include(s => s.User).FirstOrDefault(e => e.Id == id);
            if (em == null) return NotFound();

            var model = new EmployeeModel
            {
                Id = em.Id,
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
                WorkDay = em.WorkDay,
            };

            if (role?.ToLower() == "Coach")
            {
                ViewBag.Specialize = em.Specialize;
            }

            return View(model);
        }
        [HttpPost, ActionName("DeleteEmployee")]
        public async Task<IActionResult> DeleteEmployeeConfirmed(int id, string role)
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
            return RedirectToAction("Employee");
        }

        //Create coach or supervisor
        private object CreateEmployee(EmployeeModel model, string userId)
        {
            string? specialize = Request.Form["Specialize"];
            return model.Role switch
            {
                "Coach" => new Coach
                {
                    UserId = userId,
                    Status = model.Status,
                    Specialize = specialize ?? "Unknown",
                    ShiftTime = model.ShiftTime,
                    WorkDay = model.WorkDay
                },

                "Supervisor" => new Supervisor
                {
                    UserId = userId,
                    Status = model.Status,
                    ShiftTime = model.ShiftTime,
                    WorkDay = model.WorkDay
                },

                _ => throw new ArgumentException("Invalid role")
            };
        }
        //Update coach or supervisor
        private void UpdateUser(ApplicationUser user, EmployeeModel model)
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

    }
}
