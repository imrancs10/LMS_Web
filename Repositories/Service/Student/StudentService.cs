using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels.Response;

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

    public Models.Student GetStudentDetail(string RollNumber)
    {
        return (from S in _db.Student
                where S.RollNumber == RollNumber
                select S).FirstOrDefault();
    }

    public Models.Student CreateStudent(string Name, string RollNumber, string AadhaarNumber, string MobileNumber, string? FileName1, string? FileName2, string? FileName3, DateTime CreatedDate, bool IsActive, int? shiftId)
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
        if (_db.Student.FirstOrDefault(x => x.RollNumber == RollNumber && x.Name == Name) == null)
        {
            _db.Student.Add(student);
            _db.SaveChanges();
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
        _db.SaveChanges();
        return student;
    }

    public List<StudentListModel> GetStudentList()
    {
        var studentList = (from S in _db.Student
                           where S.IsActive == true
                           select S).OrderBy(x => x.Id).ToList();

        return studentList.Select(x => new StudentListModel()
        {
            Id = x.Id,
            Name = x.Name,
            RollNumber = x.RollNumber,
            AadhaarNumber = x.AadhaarNumber,
            MobileNumber = x.MobileNumber,
            CreatedDate = x.CreatedOn,
            //FileName1 = x.UploadFile1,
            //FileName2 = x.UploadFile2,
            //FileName3 = x.UploadFile3,
            //FileUrl1 = !string.IsNullOrEmpty(x.UploadFile1) ? $"{Configuration["Settings:WebsiteUrl"]}Uploads/Students/{x.RollNumber}/{x.UploadFile1}" : string.Empty,
            //FileUrl2 = !string.IsNullOrEmpty(x.UploadFile2) ? $"{Configuration["Settings:WebsiteUrl"]}Uploads/Students/{x.RollNumber}/{x.UploadFile2}" : string.Empty,
            //FileUrl3 = !string.IsNullOrEmpty(x.UploadFile3) ? $"{Configuration["Settings:WebsiteUrl"]}Uploads/Students/{x.RollNumber}/{x.UploadFile3}" : string.Empty,
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