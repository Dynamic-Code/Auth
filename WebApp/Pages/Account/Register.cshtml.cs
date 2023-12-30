using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using WebApp.Data.Account;
using WebApp.Services;
using WebApp.Settings;

namespace WebApp.Pages.Account
{
    // created this to implement register new user
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;

        public RegisterModel(UserManager<User> _userManager, IEmailService _emailService) // inject Usermanager and email service
        {
            this.userManager = _userManager;
            this.emailService = _emailService;
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
            var user = new User //  creating a new user Obj
            {
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.Email,
                Department = RegisterViewModel.Department,
                Position = RegisterViewModel.Position
            };

            // Usermanager will create the User. Usermanager Comes with identity which we need to inject
            var result = await userManager.CreateAsync(user,RegisterViewModel.Password);  // create user with password

            if (result.Succeeded) // if user creation is successfull then redirect to login
            {
                var  confirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user); // Creating a new token which we will send via email to verfy email. Also user must have a ID
                return Redirect(Url.PageLink(pageName: "/Account/ConfirmEmail",
                   values: new
                   {
                       userId = user.Id,
                       token = confirmationToken
                   }) ?? "");

                //Email Confirmation flow

                //var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail",
                //   values: new
                //   {
                //       userId = user.Id,
                //       token = confirmationToken
                //   });

                //await emailService.SendAsync("dynamic2106@gmail.com",
                //    user.Email,
                //    "Verify Email",
                //    $"Please verify email id by clicking on link :{confirmationLink}");


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

        [Required]
        public string Department { get; set; } = string.Empty;
        [Required]
        public string Position { get; set; } = string.Empty;
    }
}
