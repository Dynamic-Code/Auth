using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Auth.Pages.Account
{
    public class loggingModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; }
        public void OnGet()
        {
        }
        public void OnPost() 
        {
            if (ModelState.IsValid) return;

            // verfiy the cred
            if(Credential.UserName == "admin" && Credential.Password == "password")
            {
                // creating the security Context
            }
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
