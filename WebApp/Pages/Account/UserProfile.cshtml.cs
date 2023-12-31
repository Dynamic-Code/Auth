using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{

    //create a user profile page to let see user user their own info from claims
    public class UserProfileModel : PageModel
    {
        private readonly UserManager<User> userManager;

        [BindProperty]
        public UserProfileViewModel userProfile { get; set; }

        [BindProperty]
        public string? SuccessMessage { get; set; }

        public UserProfileModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.userProfile = new UserProfileViewModel();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            this.SuccessMessage = string.Empty;

            var (user, departmentClaim, positionClaim) = await GetUserInfoAsync();
            if (user != null) {
                this.userProfile.Email = User.Identity?.Name ?? string.Empty;
                this.userProfile.Department = departmentClaim?.Value ?? string.Empty;
                this.userProfile.Position = positionClaim?.Value ?? string.Empty;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            try
            {
                var (user, departmentClaim, positionClaim) = await GetUserInfoAsync();
                if (user != null && departmentClaim != null)
                {
                    await userManager.ReplaceClaimAsync(user, departmentClaim, new Claim(departmentClaim.Type, userProfile.Department)); //Update the profile
                }
                if (user != null && positionClaim != null)
                {
                    await userManager.ReplaceClaimAsync(user, positionClaim, new Claim(positionClaim.Type, userProfile.Position)); //Update the profile
                }
            }
            catch
            {
                ModelState.AddModelError("userProfile", "Error occured during updating user profile");
            }
            this.SuccessMessage = "User Profile is updadted successfully.";
            return Page();
        }
        private async Task<(User? user, Claim? departmentClaim, Claim? positionClaim)> GetUserInfoAsync()
        {
            var user = await userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty); //user is PageModel User
            if (user != null)
            {
                var claims = await userManager.GetClaimsAsync(user); //get claims info of the user
                var departmentClaim = claims.FirstOrDefault(x => x.Type == "Department");
                var positionClaim = claims.FirstOrDefault(x => x.Type == "Positions");

                return (user, departmentClaim, positionClaim);
            }
            else
            {
                return (null, null, null);
            }
        }
    }
    public class UserProfileViewModel
    {
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Department { get; set; } = string.Empty;
        [Required]
        public string Position { get; set; } = string.Empty;
    }
}
