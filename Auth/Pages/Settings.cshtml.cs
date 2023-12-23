using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Pages
{
    [Authorize(Policy ="AdminOnly")] // Only Admin can access this page
    public class SettingsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
