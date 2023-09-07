using LearningManagementSystem.ViewModels.Request;
using Microsoft.AspNetCore.Mvc;

namespace LearningManagementSystem.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Username != "lms@sep" && model.Username != "lms@admin")
            {
                ModelState.AddModelError(string.Empty, "Invalid Username.");
                return View(model);
            }
            else if (model.Password != "Gr8@123" && model.Password != "SPIT@54321")
            {
                ModelState.AddModelError(string.Empty, "Invalid Password.");
                return View(model);
            }

            if (model.Username == "lms@admin" && model.Password == "SPIT@54321")
            {
                HttpContext.Session.SetString("UserRole", "LMS_Admin");
                return RedirectToAction("StudentList", "Student");
            }
            else
            {
                HttpContext.Session.SetString("UserRole", "Student");
                return RedirectToAction("StudentDetails", "Student");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserRole");
            return RedirectToAction("Login", "Authentication");
        }
    }
}