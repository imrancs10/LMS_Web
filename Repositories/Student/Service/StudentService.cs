using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels.Response;

namespace LearningManagementSystem.Repositories.Student.Service;

public class StudentService : IStudentService
{
    private readonly LMSContext _db;
    private readonly IConfiguration Configuration;

    public StudentService(LMSContext db, IConfiguration configuration)
    {
        _db = db;
        Configuration = configuration;
    }

    public Models.Student GetStudentDetail(string RollNumber)
    {
        return (from S in _db.Student
                where S.RollNumber == RollNumber
                select S).FirstOrDefault();
    }

    public Models.Student CreateStudent(string Name, string RollNumber, string AadhaarNumber, string MobileNumber, string? FileName1, string? FileName2, string? FileName3, DateTime CreatedDate, bool IsActive)
    {
        Models.Student student = new Models.Student();
        student.Name = Name;
        student.RollNumber = RollNumber;
        student.AadhaarNumber = AadhaarNumber;
        student.MobileNumber = MobileNumber;
        student.UploadFile1 = !string.IsNullOrEmpty(FileName1) ? FileName1 : string.Empty;
        student.UploadFile2 = !string.IsNullOrEmpty(FileName2) ? FileName2 : string.Empty;
        student.UploadFile3 = !string.IsNullOrEmpty(FileName3) ? FileName3 : string.Empty;
        student.CreatedOn = CreatedDate;
        student.ModifiedOn = CreatedDate;
        student.IsActive = IsActive;

        _db.Student.Add(student);
        _db.SaveChanges();
        return student;
    }

    public List<StudentListModel> GetStudentList()
    {
        var studentList = (from S in _db.Student
                           where S.IsActive == true
                           select S).ToList();

        return studentList.Select(x => new StudentListModel()
        {
            Id = x.Id,
            Name = x.Name,
            RollNumber = x.RollNumber,
            AadhaarNumber = x.AadhaarNumber,
            MobileNumber = x.MobileNumber,
            CreatedDate = x.CreatedOn,
            FileUrl1 = !string.IsNullOrEmpty(x.UploadFile1) ? $"{Configuration["Settings:WebsiteUrl"]}Uploads/Students/{x.RollNumber}/{x.UploadFile1}" : string.Empty,
            FileUrl2 = !string.IsNullOrEmpty(x.UploadFile2) ? $"{Configuration["Settings:WebsiteUrl"]}Uploads/Students/{x.RollNumber}/{x.UploadFile2}" : string.Empty,
            FileUrl3 = !string.IsNullOrEmpty(x.UploadFile3) ? $"{Configuration["Settings:WebsiteUrl"]}Uploads/Students/{x.RollNumber}/{x.UploadFile3}" : string.Empty,
        }).ToList();
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