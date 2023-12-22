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
                    new Claim(ClaimTypes.Email,"admin@mywebsite.com")
                };
                // creating Identity 
                // MyCookie is the Authentication, can give any name you want
                var identity = new ClaimsIdentity(claims, "MyCookie");

                // creating claims principal which contains the Security context
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                //now we need to encrypt and serilaze the security context then it can go to cookie
                // we use HttpContext it has a SignInAsync() which is a extension method which take Authentication type and claimsprincipal

                // This will going to serialze the claimsPrincipal into it a stream and encrypt the stream to save that as a cookie in HttpContext object.

                await HttpContext.SignInAsync("MyCookie", claimsPrincipal);

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
    }
}
