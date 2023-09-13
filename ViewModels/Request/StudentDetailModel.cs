using System.ComponentModel.DataAnnotations;

namespace LearningManagementSystem.ViewModels.Request;

public class StudentDetailModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    [Display(Name = "Roll Number")]
    public string RollNumber { get; set; }

    [Required]
    [Display(Name = "Aadhaar No")]
    //[RegularExpression(@"^[2-9]{1}[0-9]{3}[0-9]{4}[0-9]{4}$", ErrorMessage = "Invalid Aadhaar Number.")]
    public string AadhaarNumber { get; set; }

    [Required]
    [Display(Name = "Mobile No")]
    [RegularExpression(@"^[1-9]\d{9}$", ErrorMessage = "Invalid Mobile Number.")]
    public string MobileNumber { get; set; }

    [Required]
    [Display(Name = "Shift")]
    public string Shift { get; set; }

    public IFormFile? UploadFile1 { get; set; }
    public IFormFile? UploadFile2 { get; set; }
    public IFormFile? UploadFile3 { get; set; }
}