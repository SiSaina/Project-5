using Microsoft.AspNetCore.Mvc;

namespace ExamProjectOne.Controllers
{
    public class AppointmentController : Controller
    {
        public IActionResult Read()
        {
            return View();
        }
    }
}
