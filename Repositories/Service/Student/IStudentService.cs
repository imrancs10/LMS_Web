using LearningManagementSystem.ViewModels.Request;
using LearningManagementSystem.ViewModels.Response;

namespace LearningManagementSystem.Repositories.Service.Student;

public interface IStudentService
{
    public StudentDetailResponseModel GetStudentDetail(string RollNumber);
    public Models.Student CreateStudent(string Name, string RollNumber, string AadhaarNumber, string MobileNumber, string? FileName1, string? FileName2, string? FileName3, DateTime CreatedDate, bool IsActive, int? Shift,int? departmentId,string StudentPhoto);
    public List<StudentListModel> GetStudentList();
    public bool DeleteStudent(int Id);
    public void UpdateStudentMark(int Id, string Mark);
}