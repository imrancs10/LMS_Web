using DocumentFormat.OpenXml.Wordprocessing;
using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels.Request;
using LearningManagementSystem.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;

namespace LearningManagementSystem.Repositories.Service.Student;

public class StudentService : IStudentService
{
    private readonly LMSContext _db;
    private readonly IConfiguration Configuration;

    public StudentService(LMSContext db, IConfiguration configuration)
    {
        _db = db;
        Configuration = configuration;
    }

    public StudentDetailResponseModel GetStudentDetail(string RollNumber)
    {
        var response = (from S in _db.Student
                        where S.RollNumber == RollNumber
                        select new StudentDetailResponseModel
                        {
                            Id = S.Id,
                            AadhaarNumber = S.AadhaarNumber,
                            RollNumber = S.RollNumber,
                            MobileNumber = S.MobileNumber,
                            Name = S.Name
                        }).FirstOrDefault();
        if (response != null)
        {
            var studentPhotoName = _db.StudentFile.FirstOrDefault(x => x.StudentId == response.Id && x.FileUploadName.Contains("_Photo"));
            var shiftName = _db.Lookup.FirstOrDefault(x => x.LookupId == studentPhotoName.ShiftId).LookupName.Replace(" ", "");
            var photoName = studentPhotoName.FileUploadName;
            var examDateEntry = _db.Lookup.FirstOrDefault(x => x.LookupType == "ExamDate").LookupName;
            var examDate = Convert.ToDateTime(examDateEntry).ToLongDateString().Replace(" ", "");
            response.StudentPhoto = $"{Configuration["Settings:WebsiteUrl"]}Uploads/Students/{examDate}/{response.RollNumber}/{shiftName}/{photoName}";
        }
        return response;
    }

    public Models.Student CreateStudent(string Name, string RollNumber, string AadhaarNumber, string MobileNumber, string? FileName1, string? FileName2, string? FileName3, DateTime CreatedDate, bool IsActive, int? shiftId, int? departmentId, string StudentPhoto)
    {
        Models.Student student = new Models.Student();
        Models.StudentFile studentFile;
        student.Name = Name;
        student.RollNumber = RollNumber;
        student.AadhaarNumber = AadhaarNumber;
        student.MobileNumber = MobileNumber;
        student.CreatedOn = CreatedDate;
        student.ModifiedOn = CreatedDate;
        student.IsActive = IsActive;
        student.DepartmentId = departmentId;
        var existingStudent = _db.Student.FirstOrDefault(x => x.RollNumber == RollNumber && x.Name == Name);
        if (existingStudent == null)
        {
            _db.Student.Add(student);
            _db.SaveChanges();
        }
        else
        {
            student = existingStudent;
        }

        if (!string.IsNullOrEmpty(FileName1))
        {
            studentFile = new Models.StudentFile();
            studentFile.ShiftId = shiftId;
            studentFile.StudentId = student.Id;
            studentFile.FileUploadName = FileName1;
            studentFile.CreatedDate = DateTime.Now;
            _db.StudentFile.Add(studentFile);
        }
        if (!string.IsNullOrEmpty(FileName2))
        {
            studentFile = new Models.StudentFile();
            studentFile.ShiftId = shiftId;
            studentFile.StudentId = student.Id;
            studentFile.FileUploadName = FileName2;
            studentFile.CreatedDate = DateTime.Now;
            _db.StudentFile.Add(studentFile);
        }
        if (!string.IsNullOrEmpty(FileName3))
        {
            studentFile = new Models.StudentFile();
            studentFile.ShiftId = shiftId;
            studentFile.StudentId = student.Id;
            studentFile.FileUploadName = FileName3;
            studentFile.CreatedDate = DateTime.Now;
            _db.StudentFile.Add(studentFile);
        }
        if (!string.IsNullOrEmpty(StudentPhoto))
        {
            studentFile = new Models.StudentFile();
            studentFile.ShiftId = shiftId;
            studentFile.StudentId = student.Id;
            studentFile.FileUploadName = StudentPhoto;
            studentFile.CreatedDate = DateTime.Now;
            _db.StudentFile.Add(studentFile);
        }
        _db.SaveChanges();
        return student;
    }

    public void UpdateStudentMark(int Id, string Mark)
    {
        var student = _db.Student.FirstOrDefault(x => x.Id == Id);
        student.Mark = Mark;
        _db.Student.Update(student);
        _db.SaveChanges();
    }

    public List<StudentListModel> GetStudentList()
    {
        var studentList = (from S in _db.Student
                           where S.IsActive == true
                           select S).OrderBy(x => x.Id).ToList();

        var shift1Id = _db.Lookup.FirstOrDefault(x => x.LookupType == "Shift" && x.LookupName == "Shift 1").LookupId;
        var shift2Id = _db.Lookup.FirstOrDefault(x => x.LookupType == "Shift" && x.LookupName == "Shift 2").LookupId;
        var examDateEntry = _db.Lookup.FirstOrDefault(x => x.LookupType == "ExamDate").LookupName;
        var examDate = Convert.ToDateTime(examDateEntry).ToLongDateString().Replace(" ", "");
        var studentData = studentList.Select(x => new StudentListModel()
        {
            Id = x.Id,
            Name = x.Name,
            RollNumber = x.RollNumber,
            AadhaarNumber = x.AadhaarNumber,
            MobileNumber = x.MobileNumber,
            CreatedDate = x.CreatedOn,
            Mark = x.Mark
        }).ToList();

        studentData.ForEach(x =>
        {
            var fileNames = _db.StudentFile.Where(y => y.StudentId == x.Id && y.ShiftId == shift1Id).Select(z => $"{Configuration["Settings:WebsiteUrl"]}Uploads/Students/{examDate}/{x.RollNumber}/Shift1/{z.FileUploadName}").ToList();
            x.Shift1FileUrls.AddRange(fileNames);
            fileNames = _db.StudentFile.Where(y => y.StudentId == x.Id && y.ShiftId == shift2Id).Select(z => $"{Configuration["Settings:WebsiteUrl"]}Uploads/Students/{examDate}/{x.RollNumber}/Shift2/{z.FileUploadName}").ToList();
            x.Shift2FileUrls.AddRange(fileNames);
        });

        return studentData;
    }

    public bool DeleteStudent(int Id)
    {
        var studentDetail = _db.Student.Where(x => x.Id == Id).FirstOrDefault();

        if (studentDetail == null)
        {
            return false;
        }

        _db.Student.Remove(studentDetail);
        _db.SaveChanges();

        return true;
    }
}