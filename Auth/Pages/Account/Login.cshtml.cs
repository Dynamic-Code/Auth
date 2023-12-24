using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;

namespace Auth.Pages.Account
{
    public class LoggingModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync() 
        {
            if (!ModelState.IsValid) return Page();

            // verfiy the cred
            if(Credential.UserName == "admin" && Credential.Password == "password")
            {
                // creating the security Context
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,"Admin"),
                    new Claim(ClaimTypes.Email,"admin@mywebsite.com"),
                    new Claim("Department","HR"), // Added a new Department claim to Authorize the HR page policy
                                                 // because HR page policy require a claim Department with HR value
                    new Claim("Admin","true"), // Added a new claim Admin to resolve AdminOnly Policy 
                    new Claim("Manager","true"), // Added a new claim to resolve ManagerClaim Policy

                    new Claim("EmploymentDate","2021-02-01") // Added a new Claim for complex Authorization 
                };
                // creating Identity 
                // MyCookie is the Authentication cookie name, can give any name you want
                var identity = new ClaimsIdentity(claims, "MyCookie");

                // creating claims principal which contains the Security context
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);


                // Added code to make the cookie persistent so that it does not get cleared after closing browser
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = Credential.RememberMe,
                };

                //now we need to encrypt and serilaze the security context then it can go to cookie
                // we use HttpContext it has a SignInAsync() which is a extension method which take Authentication type and claimsprincipal

                // This will going to serialze the claimsPrincipal into it a stream and encrypt the stream to save that as a cookie in HttpContext object.

                await HttpContext.SignInAsync("MyCookie", claimsPrincipal, authProperties);

                return RedirectToPage("/Index");

            }
            return Page();
        }    
    }

    public class Credential
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
