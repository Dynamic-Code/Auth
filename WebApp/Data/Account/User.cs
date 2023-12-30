using Microsoft.AspNetCore.Identity;

namespace WebApp.Data.Account
{
    public class User:IdentityUser // derived from IdentityUser class to add new info about user 
    {
        //Adding the new Property that we need 
        // we need to replace IdentityUser everywhere with this User class.
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }
}
