using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Pages.Account
{
    // created this to implement register new user
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;

        public RegisterModel(UserManager<IdentityUser> _userManager) // inject Usermanager
        {
            this.userManager = _userManager;
        }

        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; } = new RegisterViewModel();
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync() // when register button click
        {
            if(!ModelState.IsValid) return Page();

            //validate email address (optional  as we already chcking the uniquness of the email in program.cs identity config)

            //create the user
            var user = new IdentityUser //  creating a new user Obj
            {
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.Email
            };

            // Usermanager will create the User. Usermanager Comes with identity which we need to inject
            var result = await userManager.CreateAsync(user,RegisterViewModel.Password);  // create user with password

            if (result.Succeeded) // if user creation is successfull then redirect to login
            {
                var  confirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user); // Creating a new token which we will send via email to verfy email. Also user must have a ID

                return Redirect(Url.PageLink(pageName: "/Account/ConfirmEmail",
                    values: new
                    {
                        usrId = user.Id,
                        token = confirmationToken
                    })??""); 
                
                //return RedirectToPage("/Account/Login");
            }
            else
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }
                return Page();
            }
        }
    }
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(dataType: DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
