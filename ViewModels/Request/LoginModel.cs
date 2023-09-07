using System.ComponentModel.DataAnnotations;

namespace LearningManagementSystem.ViewModels.Request;

public class LoginModel
{
    [Required(ErrorMessage ="Enter Username")]
    public string Username { get; set;}

    [Required(ErrorMessage = "Enter Password")]
    public string Password { get; set;}
}