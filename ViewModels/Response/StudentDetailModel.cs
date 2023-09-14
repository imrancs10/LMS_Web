using System.ComponentModel.DataAnnotations;

namespace LearningManagementSystem.ViewModels.Response;

public class StudentDetailResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AadhaarNumber { get; set; }
    public string MobileNumber { get; set; }
    public int Shift { get; set; }
    public string? RollNumber { get; set; }
    public string? StudentPhoto { get; set; }
}