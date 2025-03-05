using ExamProjectOne.Data;
using ExamProjectOne.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;

namespace ExamProjectOne.Controllers
{
    public class CustomerController : Controller
    {
        public ApplicationDbContext _context { get; set; }
        public UserManager<ApplicationUser> _userManager { get; set; }
        public CustomerController(ApplicationDbContext Context, UserManager<ApplicationUser> UserManager) 
        {
            _context = Context;
            _userManager = UserManager;
        }
        public async Task<IActionResult> Customer()
        {
            var customers = await _context.Customers.Include(c => c.User).ToListAsync();

            return View(new { Customers = customers });
        }

        public IActionResult AddCustomer()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customer model)
        {
            ModelState.Remove("UserId");
            if(!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.User.UserName,
                FirstName = model.User.FirstName,
                LastName = model.User.LastName,
                Gender = model.User.Gender,
                Email = model.User.Email,
                PhoneNumber = model.User.PhoneNumber,
                DateOfBirth = model.User.DateOfBirth,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, model.User.PasswordHash);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
            var customer = new Customer
            {
                UserId = user.Id
            };

            _context.Customers.Add(customer);
            await _userManager.AddToRoleAsync(user, "Customer");
            await _context.SaveChangesAsync();

            return RedirectToAction("Customer");
        }

        public IActionResult EditCustomer(int id)
        {
            var customer = _context.Customers.Include(c => c.User).FirstOrDefault(cu => cu.Id == id);
            if (customer == null) return NotFound();
            return View(customer);
        }
        [HttpPost]
        public async Task<IActionResult> EditCustomer(Customer model)
        {
            if(!ModelState.IsValid) return View(model);

            var customer = _context.Customers
                .Include(c => c.User)
                .FirstOrDefault(c => c.Id == model.Id);

            if (customer?.User != null)
            {
                customer.User.UserName = model.User?.UserName;
                customer.User.FirstName = model.User?.FirstName ?? "";
                customer.User.LastName = model.User?.LastName ?? "";
                customer.User.Gender = model.User?.Gender ?? "";
                customer.User.Email = model.User?.Email;
                customer.User.PhoneNumber = model.User?.PhoneNumber;
                customer.User.DateOfBirth = model.User.DateOfBirth;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Customer");
        }

        public IActionResult DeleteCustomer(int id) 
        {
            var customer = _context.Customers.Include(c => c.User).FirstOrDefault(c => c.Id == id);
            return View(customer);
        }
        [HttpPost, ActionName("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomerConfirm(int id)
        {
            var customer = _context.Customers.Include(c => c.User).FirstOrDefault(c => c.Id == id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Customer");
        }
    }
}
