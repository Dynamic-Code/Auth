using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{
    // Email Confirmation Page
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<User> userManager;


        [BindProperty]
        public string Message { get; set; } = string.Empty;

        public ConfirmEmailModel(UserManager<User> _userManager) // injectig usermanager to check User
        {
            this.userManager = _userManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string token)
        {
            var user = await this.userManager.FindByIdAsync(userId); //get user
            if(user != null)
            {
               var result = await this.userManager.ConfirmEmailAsync(user, token); // validate token 
                if (result.Succeeded)
                {
                    this.Message = "Email Verified.";
                    return Page();
                }
            }
            this.Message = "Failed to validate email";
            return Page();
        }
    }
}
