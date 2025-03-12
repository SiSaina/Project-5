using ExamProjectOne.Data;
using ExamProjectOne.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ExamProjectOne.Controllers
{
    public class ServiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private Customer? _customer;
        public ServiceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Group()
        {
            var group = await _context.GroupTrainings
                .Include(s => s.Schedule).ThenInclude(c => c.Coach).ThenInclude(u => u.User)
                .Include(s => s.Schedule).ThenInclude(g => g.GymHall)
                .Include(gt => gt.GroupTrainingCustomers).ThenInclude(c => c.Customer).ThenInclude(u => u.User)
                .ToListAsync();
            var model = group.Where(gt => gt.GroupTrainingCustomers.Count < gt.Capacity).Select(g => new ScheduleModel
            {
                Id = g.Schedule.Id,
                Title = g.Schedule.Title,
                StartTime = g.Schedule.StartTime,
                EndTime = g.Schedule.EndTime,
                Date = g.Schedule.Date,
                Capacity = g.Capacity,
                CoachOne = g.Schedule.Coach,
                GymHallOne = g.Schedule.GymHall,

                Customers = g.GroupTrainingCustomers.Select(gtc => gtc.Customer).Distinct().ToList(),
            });
            return View(model);
        }
        public async Task<IActionResult> Appoint()
        {
            var model = new ScheduleModel
            {
                Coaches = await _context.Coaches.Include(u => u.User).ToListAsync(),
                GymHalls = await _context.GymHalls.ToListAsync()
            };
            return View(model);
        }

        public async Task<IActionResult> ReadCustomer()
        {
            var customer = await GetCurrentCustomerAsync();

            var group = await _context.GroupTrainings
                .Include(s => s.Schedule).ThenInclude(c => c.Coach).ThenInclude(u => u.User)
                .Include(s => s.Schedule).ThenInclude(g => g.GymHall)
                .Include(gt => gt.GroupTrainingCustomers).ThenInclude(c => c.Customer).ThenInclude(u => u.User)
                .Where(gt => gt.GroupTrainingCustomers.Any(gtc => gtc.CustomerId == customer.Id))
                .ToListAsync();

            var appoint = await _context.Appointments
                .Include(a => a.Schedule).ThenInclude(s => s.Coach).ThenInclude(u => u.User)
                .Include(a => a.Schedule).ThenInclude(s => s.GymHall)
                .Include(a => a.Customer).ThenInclude(u => u.User)
                .Where(a => a.CustomerId == customer.Id)
                .ToListAsync();

            var model = new List<ScheduleModel>();

            model.AddRange(group.Select(g => new ScheduleModel
            {
                Id = g.Schedule.Id,
                Title = g.Schedule.Title,
                StartTime = g.Schedule.StartTime,
                EndTime = g.Schedule.EndTime,
                Date = g.Schedule.Date,
                Capacity = g.Capacity,
                CoachOne = g.Schedule.Coach,
                GymHallOne = g.Schedule.GymHall,
                Customers = g.GroupTrainingCustomers.Select(gtc => gtc.Customer).ToList(),
                StatusAppoint = "Accept"
            }));

            model.AddRange(appoint.Select(a => new ScheduleModel
            {
                Id = a.Id,
                Title = a.Schedule.Title,
                StartTime = a.Schedule.StartTime,
                EndTime = a.Schedule.EndTime,
                Date = a.Schedule.Date,
                Capacity = 1,
                CoachOne = a.Schedule.Coach,
                GymHallOne = a.Schedule.GymHall,
                CustomerOne = a.Customer,
                StatusAppoint = a.Status
            }));
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Book(ScheduleModel model)
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer not found";
                return View("ReadCustomer");
            }

            var schedule = new Schedule
            {
                Title = model.Title,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Date = model.Date,
                CoachId = model.CoachId,
                GymHallId = model.GymHallId,
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            var appoint = new Appointment
            {
                ScheduleId = schedule.Id,
                CustomerId = customer.Id,
                Status = "Pending"
            };

            _context.Appointments.Add(appoint);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Waiting for coach to confirm";
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            var customer = await GetCurrentCustomerAsync();

            var groupTraining = await _context.GroupTrainings.Include(gt => gt.GroupTrainingCustomers).FirstOrDefaultAsync(gt => gt.Schedule.Id == id);
            if (groupTraining == null) return NotFound("Group training not found.");

            bool alreadyJoined = groupTraining.GroupTrainingCustomers.Any(gtc => gtc.CustomerId == customer.Id);

            if (alreadyJoined)
            {
                TempData["ErrorMessage"] = "You've already joined this group training.";
                return RedirectToAction("Group");
            }

            var join = new GroupTrainingCustomer
            {
                GroupTrainingId = groupTraining.Id,
                CustomerId = customer.Id
            };

            _context.GroupTrainingCustomers.Add(join);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Successfully joined group training!";
            return RedirectToAction("Group");
        }
        [HttpPost]
        public async Task<IActionResult> UnJoin(int id, string mode)
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return NotFound("Customer not found");
            if (mode == "Group") await HandleUnJoinGroupAsync(id, customer);
            if (mode == "Appoint") await HandleUnJoinAppointAsync(id, customer);
            TempData["SuccessMessage"] = "Action successfully";
            return RedirectToAction("ReadCustomer");
        }

        public async Task HandleUnJoinGroupAsync(int id, Customer customer) 
        {
            var groupTraining = await _context.GroupTrainings.Include(gt => gt.GroupTrainingCustomers).FirstOrDefaultAsync(gt => gt.Schedule.Id == id);
            if(groupTraining == null) return;
            
            var groupCustomer = groupTraining.GroupTrainingCustomers.FirstOrDefault(gtc => gtc.CustomerId == customer.Id);
            if(groupCustomer == null) return;

            _context.GroupTrainingCustomers.Remove(groupCustomer);
            await _context.SaveChangesAsync();
        }
        public async Task HandleUnJoinAppointAsync(int id, Customer customer) 
        {
            var appointment = await _context.Appointments.Include(a => a.Customer).FirstOrDefaultAsync(a => a.Schedule.Id == id);

            if (appointment == null) return;
            if (appointment.CustomerId != customer.Id) return;

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }

        private async Task<Customer?> GetCurrentCustomerAsync()
        {
            if (_customer != null) return _customer;

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;

            _customer = await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.UserId == user.Id);

            return _customer;
        }

    }
}
