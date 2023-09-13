using LearningManagementSystem.Repositories.Service.Lookup;
using LearningManagementSystem.Repositories.Service.Student;
using LearningManagementSystem.ViewModels.Request;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace LearningManagementSystem.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly ILookupService _lookupService;
        public AuthenticationController(ILookupService lookupService)
        {
            _lookupService = lookupService;
        }
        public IActionResult Login()
        {
            var lookupList = _lookupService.GetAllLookup();
            ViewData["Lookup"] = lookupList;
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            var lookupList = _lookupService.GetAllLookup();
            ViewData["Lookup"] = lookupList;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //check admin credential
            var adminUserName = _lookupService.GetLookupDetailByType("AdminUsername").FirstOrDefault().LookupName;
            var adminPassword = _lookupService.GetLookupDetailByType("AdminPassword").FirstOrDefault().LookupName;
            var studentPassword = _lookupService.GetLookupDetailByType("StudentPassword").FirstOrDefault().LookupName;

            if (model.Username == adminUserName && model.Password == adminPassword)
            {
                HttpContext.Session.SetString("UserRole", "LMS_Admin");
                return RedirectToAction("StudentList", "Student");
            }
            else if (model.Password == studentPassword)
            {
                HttpContext.Session.SetString("UserRole", "Student");
                HttpContext.Session.SetString("RoleNumber", model.Username);
                return RedirectToAction("StudentDetails", "Student");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid Username/Password.");
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserRole");
            return RedirectToAction("Login", "Authentication");
        }
    }
}