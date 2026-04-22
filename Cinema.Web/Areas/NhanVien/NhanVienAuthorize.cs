using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cinema.Web.Areas.NhanVien
{
    public class NhanVienAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            
            var userRole = context.HttpContext.Session.GetString("Role");

            
            if (string.IsNullOrEmpty(userRole) || (userRole != "NhanVien" && userRole != "Admin"))
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
            }

            base.OnActionExecuting(context);
        }
    }
}
