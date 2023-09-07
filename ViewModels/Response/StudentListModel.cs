namespace LearningManagementSystem.ViewModels.Response;

public class StudentListModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string RollNumber { get; set; }
    public string AadhaarNumber { get; set; }
    public string MobileNumber { get; set; }
    public string? FileUrl1 { get; set; }
    public string? FileUrl2 { get; set; }
    public string? FileUrl3 { get; set; }
    public DateTime? CreatedDate { get; set; }
}