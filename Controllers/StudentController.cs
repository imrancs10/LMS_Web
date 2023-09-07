using LearningManagementSystem.Helpers;
using LearningManagementSystem.Repositories.Student.Service;
using LearningManagementSystem.ViewModels.Request;
using Microsoft.AspNetCore.Mvc;

namespace LearningManagementSystem.Controllers;

public class StudentController : Controller
{
    private IWebHostEnvironment WebHostEnvironment { get; }
    private readonly IStudentService _studentService;

    public StudentController(IWebHostEnvironment webHostEnvironment, IStudentService studentService)
    {
        WebHostEnvironment = webHostEnvironment;
        _studentService = studentService;
    }
    public IActionResult StudentDetails()
    {
        return View();
    }

    [SessionCheck("Student")]
    [HttpPost]
    public IActionResult StudentDetails(StudentDetailModel model)
    {
        var createdDate = DateTime.Now;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var studentDetail = _studentService.GetStudentDetail(model.RollNumber);
        if (studentDetail != null)
        {
            ModelState.AddModelError(string.Empty, "Student detail with provided Desk Id already exists.");
            return View(model);
        }

        var uploadFileName1 = string.Empty;
        var uploadFileName2 = string.Empty;
        var uploadFileName3 = string.Empty;
        if (model.UploadFile1 == null)
        {
            ModelState.AddModelError("UploadFile", "File Upload 1 is mandatory");
            return View(model);
        }
        if (model.UploadFile1 != null)
        {
            string fileExtension = Path.GetExtension(model.UploadFile1.FileName);
            //if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".pdf" )
            //{
            //    ModelState.AddModelError("UploadFile", "Only jpg, jpeg and pdf files are allowed.");
            //    return View(model);
            //}
            //else
            uploadFileName1 = $"{model.RollNumber + "_1"}{fileExtension}";
        }
        if (model.UploadFile2 != null)
        {
            string fileExtension = Path.GetExtension(model.UploadFile2.FileName);
            uploadFileName2 = $"{model.RollNumber + "_2"}{fileExtension}";

        }
        if (model.UploadFile3 != null)
        {
            string fileExtension = Path.GetExtension(model.UploadFile3.FileName);
            uploadFileName3 = $"{model.RollNumber + "_3"}{fileExtension}";
        }

        var student = _studentService.CreateStudent(model.Name, model.RollNumber, model.AadhaarNumber, model.MobileNumber, uploadFileName1, uploadFileName2, uploadFileName3, createdDate, true);
        if (student != null)
        {
            if (model.UploadFile1 != null)
                UploadFile(model.UploadFile1, model.RollNumber, uploadFileName1);
            if (model.UploadFile2 != null)
                UploadFile(model.UploadFile2, model.RollNumber, uploadFileName2);
            if (model.UploadFile3 != null)
                UploadFile(model.UploadFile3, model.RollNumber, uploadFileName3);
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Something went wrong while adding student details, please contact Administrator.");
            return View(model);
        }

        ViewBag.SuccessMessage = "Student details submited successfully";
        ModelState.Clear();
        return View();
    }

    [SessionCheck("LMS_Admin")]
    [HttpGet]
    public IActionResult StudentList()
    {
        var studentList = _studentService.GetStudentList();

        if (studentList == null || studentList.Count() == 0)
        {
            ModelState.AddModelError(string.Empty, "No students added yet!");
        }

        return View(studentList);
    }

    [SessionCheck("LMS_Admin")]
    [HttpPost]
    public IActionResult DeleteStudentDetail(int id)
    {
        if (id <= 0)
        {
            ModelState.AddModelError(string.Empty, "Invalid request");
            return RedirectToAction("StudentList", "Student");
        }

        var isDeleted = _studentService.DeleteStudent(id);

        if (!isDeleted)
        {
            ModelState.AddModelError(string.Empty, "Something went wrong while deleting student detail, please contact Administrator!");
        }

        return RedirectToAction("StudentList", "Student");
    }

    #region Private Methods

    private string UploadFile(IFormFile file, string rollNumber, string uploadFileName)
    {
        string uploadsFolder = Path.Combine(WebHostEnvironment.WebRootPath, $"Uploads/Students/{rollNumber}");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        string filePath = Path.Combine(uploadsFolder, uploadFileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(fileStream);
        }

        return file.FileName;
    }

    #endregion Private Methods
}
