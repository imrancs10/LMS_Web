using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LearningManagementSystem.Helpers;

public class SessionCheckAttribute : ActionFilterAttribute
{
    private readonly string[] allowedRoles;

    public SessionCheckAttribute(params string[] roles)
    {
        this.allowedRoles = roles;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var sessionRole = context.HttpContext.Session.GetString("UserRole");

        if (!IsUserInAllowedRoles(sessionRole))
        {
            context.Result = new RedirectToActionResult("Login", "Authentication", null);
        }

        base.OnActionExecuting(context);
    }

    private bool IsUserInAllowedRoles(string userRole)
    {
        if (string.IsNullOrEmpty(userRole))
        {
            return false;
        }

        foreach (var allowedRole in allowedRoles)
        {
            if (userRole.Equals(allowedRole, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}