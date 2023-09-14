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
    public List<string> Shift1FileUrls { get; set; }
    public List<string> Shift2FileUrls { get; set; }
    public DateTime? CreatedDate { get; set; }
}