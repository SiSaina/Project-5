using Microsoft.AspNetCore.Mvc;
using ExamProjectOne.Models;
using ExamProjectOne.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Linq;

namespace ExamProjectOne.Controllers
{
    public class ScheduleController : Controller
    {
        private List<Schedule> Schedules { get; set; }
        private List<GroupTraining> GroupTrainings { get; set; }
        private ApplicationDbContext _context;
        public ScheduleController(ApplicationDbContext Context) 
        {
            Schedules = [];
            GroupTrainings = [];
            _context = Context;
        }

        public async Task<IActionResult> Read()
        {
            GroupTrainings = await _context.GroupTrainings
                .Include(s => s.Schedule)
                    .ThenInclude(c => c.Coach)
                    .ThenInclude(u => u.User)
                .Include(s => s.Schedule)
                    .ThenInclude(g => g.GymHall)
                .Include(gt => gt.GroupTrainingCustomers)
                    .ThenInclude(c => c.Customer)
                    .ThenInclude(u => u.User)
                .ToListAsync();

            var ScheduleModels = GroupTrainings
                .GroupBy(gt => gt.Schedule)
                .Select(g => new ScheduleModel
                {
                    Id = g.Key.Id,
                    Title = g.Key.Title,
                    StartTime = g.Key.StartTime,
                    EndTime = g.Key.EndTime,
                    Date = g.Key.Date,
                    Capacity = g.First().Capacity,
                    Coach = g.Key.Coach,
                    GymHall = g.Key.GymHall,

                    Customers = g.SelectMany(gt => gt.GroupTrainingCustomers.Select(gtc => gtc.Customer)).Distinct().ToList(),
                }).ToList();

            return View(ScheduleModels);
        }

        public IActionResult Create()
        {
            var model = new ScheduleModel
            {
                Coaches = _context.Coaches.Include(c => c.User).ToList(),
                GymHalls = _context.GymHalls.ToList()
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ScheduleModel model)
        {
            ModelState.Remove("CoachName");
            ModelState.Remove("GymHallName");
            ModelState.Remove("SelectCustomer");
            if (!ModelState.IsValid) return View(model);

            bool isOverlapping = await _context.Schedules
                .AnyAsync(s => s.CoachId == model.CoachId &&
                s.Date == model.Date &&
                (model.StartTime < s.EndTime && model.EndTime > s.StartTime));

            if (isOverlapping) return View(model);

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
            var groupTraining = new GroupTraining
            {
                ScheduleId = schedule.Id,
                Capacity = model.Capacity
            };

            _context.GroupTrainings.Add(groupTraining);

            await _context.SaveChangesAsync();
            return RedirectToAction("Select", new { groupTrainingId = groupTraining.Id, capacity = model.Capacity, action = "Select" });
        }

        public async Task<IActionResult> Select(int groupTrainingId, int capacity, string mode)
        {
            var customers = await _context.Customers.Include(c => c.User).ToListAsync();

            List<int?> existCustomer = [];
            if(groupTrainingId > 0)
            {
                existCustomer = await _context.GroupTrainingCustomers
                        .Where(gtc => gtc.GroupTrainingId == groupTrainingId)
                        .Select(gtc => gtc.CustomerId)
                        .ToListAsync();
            }
            var model = new ScheduleModel
            {
                Id = groupTrainingId,
                Capacity = capacity,
                Customers = customers,
                SelectCustomer = existCustomer,
                Action = mode
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> SelectOrUpdate(List<int> selectCustomer, ScheduleModel model, int Capacity, int groupTrainingId, string action)
        {
            if (action == "Select") await HandleSelect(selectCustomer, Capacity, groupTrainingId);
            if (action == "Update") await HandleUpdate(selectCustomer, Capacity, groupTrainingId, model);

            return RedirectToAction("Read");
        }

        public async Task<IActionResult> Update(int id)
        {
            var gt = await _context.GroupTrainings
                .Include(gt => gt.Schedule)
                    .ThenInclude(s => s.Coach)
                    .ThenInclude(u => u.User)
                .Include(gt => gt.Schedule)
                    .ThenInclude(s => s.GymHall)
                .Include(gt => gt.GroupTrainingCustomers)
                    .ThenInclude(gtc => gtc.Customer)
                    .ThenInclude(u => u.User)
                .FirstOrDefaultAsync(gt => gt.Schedule.Id == id);

            var coaches = await _context.Coaches.Include(c => c.User).ToListAsync();
            var gymHalls = await _context.GymHalls.ToListAsync();

            var scheduleModel = new ScheduleModel
            {
                Id = gt.Schedule.Id,
                Title = gt.Schedule.Title,
                StartTime = gt.Schedule.StartTime,
                EndTime = gt.Schedule.EndTime,
                Date = gt.Schedule.Date,
                Capacity = gt.Capacity,

                CoachId = gt.Schedule.CoachId,
                GymHallId = gt.Schedule.GymHallId,

                Customers = gt.GroupTrainingCustomers.Select(gtc => gtc.Customer).ToList(),
                Coaches = coaches,
                GymHalls = gymHalls,

                CoachName = gt.Schedule.Coach.User.UserName,
                GymHallName = gt.Schedule.GymHall.Name,

            };
            return View(scheduleModel);
        }
        [HttpPost]
        public async Task<IActionResult> Update(ScheduleModel model)
        {
            ModelState.Remove("CoachName");
            ModelState.Remove("GymHallName");
            ModelState.Remove("SelectCustomer");
            if (!ModelState.IsValid) return View(model);

            var schedule = await _context.Schedules.FindAsync(model.Id);
            if (schedule == null) return NotFound();

            bool isOverlapping = await _context.Schedules
                .AnyAsync(s => s.CoachId == model.CoachId &&
                               s.Id != model.Id &&
                               s.Date == model.Date &&
                               (model.StartTime < s.EndTime && model.EndTime > s.StartTime));

            if (isOverlapping) return View(model);

            schedule.Title = model.Title;
            schedule.StartTime = model.StartTime;
            schedule.EndTime = model.EndTime;
            schedule.Date = model.Date;
            schedule.CoachId = model.CoachId;
            schedule.GymHallId = model.GymHallId;

            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();

            var groupTraining = await _context.GroupTrainings.FirstOrDefaultAsync(gt => gt.ScheduleId == model.Id);
            if (groupTraining != null)
            {
                groupTraining.Capacity = model.Capacity;
                _context.GroupTrainings.Update(groupTraining);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Select", new { groupTrainingId = groupTraining?.Id, capacity = model.Capacity, mode = "Update" });
        }

        public async Task<IActionResult> HandleSelect(List<int> selectCustomer, int Capacity, int groupTrainingId)
        {
            if (selectCustomer != null && selectCustomer.Count != 0)
            {
                if (selectCustomer.Count > Capacity)
                {
                    ModelState.AddModelError("", $"You can only select up to {Capacity} customers. Please select fewer customers.");
                    return View();
                }
                var groupTrainings = selectCustomer.Select(customerId => new GroupTrainingCustomer
                {
                    CustomerId = customerId,
                    GroupTrainingId = groupTrainingId,
                }).ToList();

                _context.GroupTrainingCustomers.AddRange(groupTrainings);
                await _context.SaveChangesAsync();

                return RedirectToAction("Read");
            }

            return View();
        }
        public async Task<IActionResult> HandleUpdate(List<int> selectCustomer, int Capacity, int groupTrainingId, ScheduleModel model)
        {
            if (selectCustomer != null && selectCustomer.Count > 0)
            {
                if (selectCustomer.Count > Capacity)
                {
                    ModelState.AddModelError("", $"You can only select up to {Capacity} customers. Please select fewer customers.");
                    model.Customers = await _context.Customers.Include(c => c.User).ToListAsync();
                    return View(model);
                }

                var existingGroupCustomers = _context.GroupTrainingCustomers.Where(gtc => gtc.GroupTrainingId == groupTrainingId);

                _context.GroupTrainingCustomers.RemoveRange(existingGroupCustomers.Where(gtc => !selectCustomer.Contains(gtc.CustomerId.Value)));
                var existingCustomerIds = existingGroupCustomers.Select(gtc => gtc.CustomerId).ToList();
                var newCustomers = selectCustomer.Where(id => !existingCustomerIds.Contains(id))
                    .Select(id => new GroupTrainingCustomer
                    {
                        CustomerId = id,
                        GroupTrainingId = groupTrainingId
                    }).ToList();

                _context.GroupTrainingCustomers.AddRange(newCustomers);
                await _context.SaveChangesAsync();

                return RedirectToAction("Read");
            }

            ModelState.AddModelError("", "You must select at least one customer.");
            model.Customers = await _context.Customers.Include(c => c.User).ToListAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return NotFound();
            var relatedGroupTrainings = await _context.GroupTrainings.Where(gt => gt.ScheduleId == id).ToListAsync();

            _context.GroupTrainings.RemoveRange(relatedGroupTrainings);
            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction("Read");
        }
    }
}
