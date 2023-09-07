using LearningManagementSystem.ViewModels.Response;

namespace LearningManagementSystem.Repositories.Student.Service;

public interface IStudentService
{
    public Models.Student GetStudentDetail(string RollNumber);
    public Models.Student CreateStudent(string Name, string RollNumber, string AadhaarNumber, string MobileNumber, string? FileName1, string? FileName2, string? FileName3, DateTime CreatedDate, bool IsActive);
    public List<StudentListModel> GetStudentList();
    public bool DeleteStudent(int Id);
}