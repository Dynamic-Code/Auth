using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
           await HttpContext.SignOutAsync("MyCookie"); //Remove the cookie to signout
           return RedirectToPage("/index");
        }
    }
}
