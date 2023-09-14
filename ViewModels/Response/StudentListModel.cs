namespace LearningManagementSystem.ViewModels.Response;

public class StudentListModel
{
    public StudentListModel()
    {
        Shift1FileUrls = new List<string>();
        Shift2FileUrls = new List<string>();
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public string RollNumber { get; set; }
    public string AadhaarNumber { get; set; }
    public string MobileNumber { get; set; }
    public string Mark { get; set; }
    public string? FileUrl1 { get; set; }
    public string? FileUrl2 { get; set; }
    public string? FileUrl3 { get; set; }
    public string? FileName1 { get; set; }
    public string? FileName2 { get; set; }
    public string? FileName3 { get; set; }
    public List<string> Shift1FileUrls { get; set; }
    public List<string> Shift2FileUrls { get; set; }
    public DateTime? CreatedDate { get; set; }
}