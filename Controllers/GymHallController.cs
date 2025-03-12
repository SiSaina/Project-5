using ExamProjectOne.Data;
using ExamProjectOne.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamProjectOne.Controllers
{
    [Authorize(Roles = "Admin, Senior supervisor, Senior coach, Supervisor, Coach")]
    public class GymHallController : Controller
    {
        public ApplicationDbContext _context { get; set; }
        public GymHallController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Read()
        {
            var gymHall = await _context.GymHalls.ToListAsync();
            return View(new { GymHall = gymHall });
        }

        public IActionResult Upsert(int? id)
        {
            if (id == null) return View(new GymHall());
            var gymHall = _context.GymHalls.Find(id);
            if (gymHall == null) return NotFound();
            return View(gymHall);
        }
        [HttpPost]
        public async Task<IActionResult> Upsert(GymHall gymHall)
        {
            if (ModelState.IsValid)
            {
                if (gymHall.Id == 0) _context.GymHalls.Add(gymHall);
                else _context.GymHalls.Update(gymHall);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Action completed successfully";
                return RedirectToAction("Read");
            }
            return View(gymHall);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var gymHall = _context.GymHalls.Find(id);
            if (gymHall != null)
            {
                _context.GymHalls.Remove(gymHall);
                await _context.SaveChangesAsync();
            }
            TempData["SuccessMessage"] = "Delete successfully";
            return RedirectToAction("Read");
        }
    }
}
