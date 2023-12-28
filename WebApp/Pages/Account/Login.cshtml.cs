using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Pages.Account
{
    public class LoggingModel : PageModel
    {
        private readonly SignInManager<IdentityUser> signInManager;

        public LoggingModel(SignInManager<IdentityUser> _signInManager) // inject SignInManager
        {
            this.signInManager = _signInManager;
        }

        [BindProperty]
        public CredentialViewModel Credential { get; set; } = new CredentialViewModel();
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync() // on login button click
        {
            if(!ModelState.IsValid) return Page();

            var result = await signInManager.PasswordSignInAsync(   // provided by identity for signin 
                 this.Credential.Email,
                 this.Credential.Password,
                 this.Credential.RememberMe,
                 false);

            if(result.Succeeded)
            {
                return RedirectToPage("/Index");    
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login", "You are logged out");
                }
                else
                {
                    ModelState.AddModelError("Login", "Failed to Logn");
                }
            }
            return Page(); 

        }    
    }

    public class CredentialViewModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(dataType: DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
