using ExamProjectOne.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExamProjectOne.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SwitchRole(string role)
        {
            GlobalRoleManager.SetRole(role);
            return RedirectToAction("Index", "Home");
        }

    }
}
