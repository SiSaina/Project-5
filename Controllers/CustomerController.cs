﻿using ExamProjectOne.Data;
using ExamProjectOne.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;

namespace ExamProjectOne.Controllers
{
    [Authorize(Roles = "Admin, Senior supervisor, Senior coach, Supervisor, Coach")]
    public class CustomerController : Controller
    {
        public ApplicationDbContext _context { get; set; }
        public UserManager<ApplicationUser> _userManager { get; set; }
        public CustomerController(ApplicationDbContext Context, UserManager<ApplicationUser> UserManager) 
        {
            _context = Context;
            _userManager = UserManager;
        }
        public async Task<IActionResult> Read()
        {
            var customers = await _context.Customers.OrderBy(c => c.Id).Include(c => c.User).ToListAsync();

            return View(new { Customers = customers });
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserModel model)
        {
            ModelState.Remove("Role");
            if(!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.BirthDate,
                EmailConfirmed = true,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Fail to create";
                return RedirectToAction("Read");
            }
            var customer = new Customer { UserId = user.Id };

            _context.Customers.Add(customer);
            await _userManager.AddToRoleAsync(user, "Customer");
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Created successfully";
            return RedirectToAction("Read");
        }

        public IActionResult Update(int id)
        {
            var customer = _context.Customers.Include(c => c.User).FirstOrDefault(cu => cu.Id == id);
            if (customer == null) return NotFound();
            return View(customer);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Customer model)
        {
            ModelState.Remove("UserId");
            if(!ModelState.IsValid) return View(model);

            var customer = _context.Customers.Include(c => c.User).FirstOrDefault(c => c.Id == model.Id);

            if (customer?.User != null)
            {
                customer.User.UserName = model.User.UserName;
                customer.User.FirstName = model.User.FirstName ?? "";
                customer.User.LastName = model.User.LastName ?? "";
                customer.User.Gender = model.User.Gender ?? "";
                customer.User.Email = model.User.Email;
                customer.User.PhoneNumber = model.User.PhoneNumber;
                customer.User.DateOfBirth = model.User.DateOfBirth;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Update successfully";
            return RedirectToAction("Read");
        }

        public IActionResult Delete(int id) 
        {
            var customer = _context.Customers.Include(c => c.User).FirstOrDefault(c => c.Id == id);
            return View(customer);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = _context.Customers.Include(c => c.User).FirstOrDefault(c => c.Id == id);
            if (customer == null) return View("Error");
            var user = customer?.User;
            if (user == null) return View("Error");
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count > 1)
            {
                await _userManager.RemoveFromRoleAsync(user, "Customer");
            }
            else
            {
                _context.Users.Remove(user);
            }
            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Delete successfully";
            return RedirectToAction("Read");
        }

    }
}
