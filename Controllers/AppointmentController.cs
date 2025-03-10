using ExamProjectOne.Data;
using ExamProjectOne.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ExamProjectOne.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin, Senior supervisor, Senior coach, Supervisor, Coach")]
        public async Task<IActionResult> Read()
        {
            var userId = _userManager.GetUserId(User);
            var appoint = await GetAppointmentsAsync(userId);

            var model = appoint.Select(a => new ScheduleModel
            {
                Id = a.Id,
                Title = a.Schedule.Title,
                StartTime = a.Schedule.StartTime,
                EndTime = a.Schedule.EndTime,
                Date = a.Schedule.Date,
                CustomerOne = a.Customer,
                CoachOne = a.Schedule.Coach,
                GymHallOne = a.Schedule.GymHall
            }).ToList();

            return View(model);
        }


        // -------- Create --------

        [Authorize(Roles = "Admin, Senior supervisor, Senior coach, Supervisor, Coach")]
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);
            var model = await GetScheduleModelAsync(null, userId);
            return View(model);
        }
        [Authorize(Roles = "Admin, Senior supervisor, Senior coach, Supervisor, Coach")]
        [HttpPost]
        public async Task<IActionResult> Create(ScheduleModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = _userManager.GetUserId(User);
            var coach = await _context.Coaches.Include(u => u.User).FirstOrDefaultAsync(c => c.UserId == userId);

            if (coach != null) await HandleCreate(model, coach.Id);
            if (coach == null) await HandleCreate(model, model.CoachId);

            return RedirectToAction("Read");
        }


        // -------- Update --------

        [Authorize(Roles = "Admin, Senior supervisor, Senior coach, Supervisor, Coach")]
        public async Task<IActionResult> Update(int id)
        {
            var model = await GetScheduleModelAsync(id);
            if (model.Customers == null) return View();
            return View(model);
        }
        [Authorize(Roles = "Admin, Senior supervisor, Senior coach, Supervisor, Coach")]
        [HttpPost]
        public async Task<IActionResult> Update(ScheduleModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var appoint = await _context.Appointments
                .Include(c => c.Customer)
                    .ThenInclude(u => u.User)
                .Include(s => s.Schedule)
                    .ThenInclude(g => g.GymHall)
                .Include(s => s.Schedule)
                    .ThenInclude(c => c.Coach)
                        .ThenInclude(u => u.User)
                .FirstOrDefaultAsync(s => s.Id == model.Id);

            if (appoint != null)
            {
                appoint.Schedule.Title = model.Title;
                appoint.Schedule.StartTime = model.StartTime;
                appoint.Schedule.EndTime = model.EndTime;
                appoint.Schedule.Date = model.Date;
                appoint.Schedule.CoachId = model.CoachId;
                appoint.Schedule.GymHallId = model.GymHallId;
                appoint.CustomerId = model.CustomerId;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Read");
        }


        // -------- Delete --------

        [Authorize(Roles = "Admin, Senior supervisor, Senior coach, Supervisor, Coach")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var appoint = await _context.Appointments.FindAsync(id);
            if (appoint == null) return NotFound();

            _context.Appointments.Remove(appoint);
            await _context.SaveChangesAsync();

            return RedirectToAction("Read");
        }

        private async Task<List<Appointment>> GetAppointmentsAsync(string id)
        {
            var query = _context.Appointments
                .Include(c => c.Customer).ThenInclude(u => u.User)
                .Include(s => s.Schedule).ThenInclude(g => g.GymHall)
                .Include(s => s.Schedule).ThenInclude(c => c.Coach).ThenInclude(u => u.User)
                .AsQueryable();

            if (User.IsInRole("Coach"))
            {
                query = query.Where(a => a.Schedule.Coach.UserId == id);
            }
            return await query.ToListAsync();
        }
        private async Task<ScheduleModel> GetScheduleModelAsync(int? appointmentId = null, string? userId = null)
        {
            var model = new ScheduleModel
            {
                Customers = _context.Customers.Include(u => u.User).ToList(),
                GymHalls = _context.GymHalls.ToList(),
            };

            if (userId != null && User.IsInRole("Coach"))
            {
                var coach = await _context.Coaches.Include(u => u.User).FirstOrDefaultAsync(c => c.UserId == userId);
                if (coach != null) model.Coaches = [coach];
            }
            else model.Coaches = _context.Coaches.Include(u => u.User).ToList();

            if (appointmentId.HasValue)
            {
                var appoint = await _context.Appointments
                    .Include(c => c.Customer).ThenInclude(u => u.User)
                    .Include(s => s.Schedule).ThenInclude(g => g.GymHall)
                    .Include(s => s.Schedule).ThenInclude(c => c.Coach).ThenInclude(u => u.User)
                    .FirstOrDefaultAsync(a => a.Id == appointmentId);

                if (appoint?.Schedule != null)
                {
                    model.Title = appoint.Schedule.Title;
                    model.StartTime = appoint.Schedule.StartTime;
                    model.EndTime = appoint.Schedule.EndTime;
                    model.Date = appoint.Schedule.Date;
                    model.CustomerId = appoint.CustomerId ?? 0;
                    model.CoachId = appoint.Schedule.CoachId;
                    model.GymHallId = appoint.Schedule.GymHallId;
                }
            }

            return model;
        }
        public async Task HandleCreate(ScheduleModel model, int id)
        {

            var schedule = new Schedule
            {
                Title = model.Title,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Date = model.Date,
                CoachId = id,
                GymHallId = model.GymHallId,
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            var appoint = new Appointment
            {
                ScheduleId = schedule.Id,
                CustomerId = model.CustomerId
            };

            _context.Appointments.Add(appoint);
            await _context.SaveChangesAsync();
        }
    }
}
