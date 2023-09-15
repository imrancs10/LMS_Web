using ClosedXML.Excel;
using iText.Html2pdf;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Repositories.Service.Lookup;
using LearningManagementSystem.Repositories.Service.Student;
using LearningManagementSystem.ViewModels.Request;
using LearningManagementSystem.ViewModels.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Reflection;
using System.Text;
using static Azure.Core.HttpHeader;
using Path = System.IO.Path;

namespace LearningManagementSystem.Controllers;

public class StudentController : Controller
{
    private IWebHostEnvironment WebHostEnvironment { get; }
    private readonly IStudentService _studentService;
    private readonly ILookupService _lookupService;

    public StudentController(IWebHostEnvironment webHostEnvironment, IStudentService studentService, ILookupService lookupService)
    {
        _lookupService = lookupService;
        WebHostEnvironment = webHostEnvironment;
        _studentService = studentService;
    }
    public IActionResult StudentDetails(string? msg)
    {
        var lookupList = _lookupService.GetAllLookup();
        ViewData["Lookup"] = lookupList;
        var RollNumber = HttpContext.Session.GetString("RoleNumber");
        var studentDetail = _studentService.GetStudentDetail(RollNumber);
        ViewData["Student"] = studentDetail != null ? studentDetail : new StudentDetailResponseModel();
        if (!string.IsNullOrEmpty(msg))
        {
            ViewBag.SuccessMessage = msg;
        }
        return View();
    }

    [SessionCheck("Student")]
    [HttpPost]
    public IActionResult StudentDetails(StudentDetailModel model)
    {
        var lookupList = _lookupService.GetAllLookup();
        ViewData["Lookup"] = lookupList;
        model.RollNumber = HttpContext.Session.GetString("RoleNumber");
        var studentDetail = _studentService.GetStudentDetail(model.RollNumber);
        ViewData["Student"] = studentDetail != null ? studentDetail : new StudentDetailResponseModel();
        var createdDate = DateTime.Now;

        if (!ModelState.IsValid)
        {
            return View(model);
        }
        if (studentDetail != null)
        {
            //ModelState.AddModelError(string.Empty, "Student detail with provided Desk Id already exists.");
            //return View(model);
        }

        var departmentId = Convert.ToInt32(HttpContext.Session.GetString("DepartmentId"));

        var uploadFileName1 = string.Empty;
        var uploadFileName2 = string.Empty;
        var uploadFileName3 = string.Empty;
        var StudentPhoto = string.Empty;
        //if (model.UploadPhoto == null)
        //{
        //    ModelState.AddModelError("UploadFile", "Photo is mandatory");
        //    return View(model);
        //}
        //if (model.UploadFile1 == null)
        //{
        //    ModelState.AddModelError("UploadFile", "File Upload 1 is mandatory");
        //    return View(model);
        //}
        if (model.Shift == 0)
        {
            ModelState.AddModelError("Shift", "Shift is mandatory");
            return View(model);
        }
        var shiftName = lookupList.FirstOrDefault(x => x.LookupId == model.Shift).LookupName;
        shiftName = shiftName.Replace(" ", "");
        if (model.UploadPhoto != null)
        {
            string fileExtension = Path.GetExtension(model.UploadPhoto.FileName);
            StudentPhoto = $"{model.RollNumber + "_Photo"}{fileExtension}";
        }
        if (model.UploadFile1 != null)
        {
            string fileExtension = Path.GetExtension(model.UploadFile1.FileName);
            uploadFileName1 = $"{model.RollNumber + "_File1"}{fileExtension}";
        }
        if (model.UploadFile2 != null)
        {
            string fileExtension = Path.GetExtension(model.UploadFile2.FileName);
            uploadFileName2 = $"{model.RollNumber + "_File2"}{fileExtension}";

        }
        if (model.UploadFile3 != null)
        {
            string fileExtension = Path.GetExtension(model.UploadFile3.FileName);
            uploadFileName3 = $"{model.RollNumber + "_File3"}{fileExtension}";
        }

        var student = _studentService.CreateStudent(model.Name, model.RollNumber, model.AadhaarNumber, model.MobileNumber, uploadFileName1, uploadFileName2, uploadFileName3, createdDate, true, model.Shift, departmentId, StudentPhoto);
        if (student != null)
        {
            var dateOfExam = lookupList.FirstOrDefault(x => x.LookupType == "ExamDate").LookupName;
            var examDate = Convert.ToDateTime(dateOfExam).ToLongDateString().Replace(" ", "");
            if (model.UploadPhoto != null)
                UploadFile(model.UploadPhoto, model.RollNumber, StudentPhoto, shiftName, examDate);

            if (model.UploadFile1 != null)
                UploadFile(model.UploadFile1, model.RollNumber, uploadFileName1, shiftName, examDate);
            if (model.UploadFile2 != null)
                UploadFile(model.UploadFile2, model.RollNumber, uploadFileName2, shiftName, examDate);
            if (model.UploadFile3 != null)
                UploadFile(model.UploadFile3, model.RollNumber, uploadFileName3, shiftName, examDate);

            if (model.UploadFile1 != null || model.UploadFile2 != null || model.UploadFile3 != null)
            {
               return RedirectToAction("Logout", "Authentication", new { msg = "Student details submited successfully" });
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Something went wrong while adding student details, please contact Administrator.");
            return View(model);
        }

        //ViewBag.SuccessMessage = "Student details submited successfully";
        //ModelState.Clear();
        return RedirectToAction("StudentDetails", new { msg = "Student details submited successfully" });
    }

    [SessionCheck("LMS_Admin")]
    [HttpGet]
    public IActionResult StudentList()
    {
        var lookupList = _lookupService.GetAllLookup();
        ViewData["Lookup"] = lookupList;
        var studentList = _studentService.GetStudentList();

        if (studentList == null || studentList.Count() == 0)
        {
            ModelState.AddModelError(string.Empty, "No students added yet!");
        }

        return View(studentList);
    }

    [SessionCheck("LMS_User")]
    [HttpGet]
    public IActionResult UserStudentList()
    {
        var lookupList = _lookupService.GetAllLookup();
        ViewData["Lookup"] = lookupList;
        var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
        var totalUser = _lookupService.TotalUserDetail();
        var userCount = totalUser.Count();
        var studentList = _studentService.GetStudentList();
        var recordCount = studentList.Count();
        var UserIndex = totalUser.FindIndex(x => x.UserId == userId);
        var recordDevideCount = recordCount / userCount;

        if (UserIndex != userCount - 1)
            studentList = studentList.Skip(UserIndex).Take(recordDevideCount).ToList();
        else
        {
            recordDevideCount = recordDevideCount + recordCount % userCount;
            studentList = studentList.Skip(UserIndex).Take(recordDevideCount).ToList();
        }


        if (studentList == null || studentList.Count() == 0)
        {
            ModelState.AddModelError(string.Empty, "No students added yet!");
        }

        return View(studentList);
    }


    public IActionResult SubmitMark(int id, string mark)
    {
        _studentService.UpdateStudentMark(id, mark);
        return RedirectToAction("UserStudentList", "Student");
    }

    [SessionCheck("LMS_Admin")]
    [HttpPost]
    public IActionResult DeleteStudentDetail(int id)
    {
        var lookupList = _lookupService.GetAllLookup();
        ViewData["Lookup"] = lookupList;
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
    [HttpPost]
    public FileResult Export()
    {
        var lookupList = _lookupService.GetAllLookup();
        ViewData["Lookup"] = lookupList;
        List<StudentListModel> customers = _studentService.GetStudentList();

        if (customers == null || customers.Count() == 0)
        {
            ModelState.AddModelError(string.Empty, "No students added yet!");
        }

        //Building an HTML string.
        StringBuilder sb = new StringBuilder();

        //Table start.
        sb.Append("<table border='1' cellpadding='5' cellspacing='0' style='border: 1px solid #ccc;font-family: Arial; font-size: 10pt;'>");

        //Building the Header row.
        sb.Append("<tr>");
        sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Sr No</th>");
        sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Name</th>");
        sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>System Number</th>");
        sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Govt. Id Number</th>");
        sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Mobile No</th>");
        //sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>File Name 1</th>");
        //sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>File Name 2</th>");
        //sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>File Name 3</th>");
        sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Mark</th>");
        sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Submitted On</th>");
        sb.Append("</tr>");

        //Building the Data rows.
        for (int i = 0; i < customers.Count; i++)
        {
            var customer = customers[i];
            sb.Append("<tr>");
            sb.Append("<td style='border: 1px solid #ccc'>");
            sb.Append((i + 1).ToString());
            sb.Append("</td>");

            sb.Append("<td style='border: 1px solid #ccc'>");
            sb.Append(customer.Name);
            sb.Append("</td>");

            sb.Append("<td style='border: 1px solid #ccc'>");
            sb.Append(customer.RollNumber);
            sb.Append("</td>");

            sb.Append("<td style='border: 1px solid #ccc'>");
            sb.Append(customer.AadhaarNumber);
            sb.Append("</td>");

            sb.Append("<td style='border: 1px solid #ccc'>");
            sb.Append(customer.MobileNumber);
            sb.Append("</td>");

            sb.Append("<td style='border: 1px solid #ccc'>");
            sb.Append(customer.Mark);
            sb.Append("</td>");

            //sb.Append("<td style='border: 1px solid #ccc'>");
            //sb.Append(customer.FileName1);
            //sb.Append("</td>");

            //sb.Append("<td style='border: 1px solid #ccc'>");
            //sb.Append(customer.FileName2);
            //sb.Append("</td>");

            //sb.Append("<td style='border: 1px solid #ccc'>");
            //sb.Append(customer.FileName3);
            //sb.Append("</td>");

            sb.Append("<td style='border: 1px solid #ccc'>");
            sb.Append(customer.CreatedDate.Value.ToString());
            sb.Append("</td>");
            sb.Append("</tr>");
        }

        //Table end.
        sb.Append("</table>");

        using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(sb.ToString())))
        {
            ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
            PdfWriter writer = new PdfWriter(byteArrayOutputStream);
            PdfDocument pdfDocument = new PdfDocument(writer);
            pdfDocument.SetDefaultPageSize(PageSize.A4.Rotate());
            HtmlConverter.ConvertToPdf(stream, pdfDocument);
            pdfDocument.Close();
            return File(byteArrayOutputStream.ToArray(), "application/pdf", "Student_Export.pdf");
        }
    }
    [HttpPost]
    public IActionResult ExportExcel()
    {
        var lookupList = _lookupService.GetAllLookup();
        ViewData["Lookup"] = lookupList;
        List<StudentListModel> customers = _studentService.GetStudentList();
        var dataToExport = (from x in customers
                            select new
                            {
                                Name = x.Name,
                                RollNumber = x.RollNumber,
                                AadhaarNumber = x.AadhaarNumber,
                                MobileNumber = x.MobileNumber,
                                Mark = x.Mark,
                                CreatedDate = x.CreatedDate.Value.ToLongDateString(),
                            }).ToList();
        using (XLWorkbook wb = new XLWorkbook())
        {
            wb.Worksheets.Add(ToDataTable(dataToExport.ToList()));
            using (MemoryStream stream = new MemoryStream())
            {
                wb.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Student_Export.xlsx");
            }
        }
        return View();
    }
    #region Private Methods
    public DataTable ToDataTable<T>(List<T> items)
    {
        DataTable dataTable = new DataTable(typeof(T).Name);
        //Get all the properties
        PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo prop in Props)
        {
            //Setting column names as Property names
            dataTable.Columns.Add(prop.Name);
        }
        foreach (T item in items)
        {
            var values = new object[Props.Length];
            for (int i = 0; i < Props.Length; i++)
            {
                //inserting property values to datatable rows
                values[i] = Props[i].GetValue(item, null);
            }
            dataTable.Rows.Add(values);
        }
        //put a breakpoint here and check datatable
        return dataTable;
    }
    private void UploadFile(IFormFile file, string rollNumber, string uploadFileName, string shiftName, string examDate)
    {
        string uploadsFolder = Path.Combine(WebHostEnvironment.WebRootPath, $"Uploads/Students/{examDate}/{rollNumber}/{shiftName}");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        string filePath = Path.Combine(uploadsFolder, uploadFileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(fileStream);
        }
        //return file.FileName;
    }

    #endregion Private Methods
}
