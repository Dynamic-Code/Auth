using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Pages
{
    [Authorize(Policy = "MustBelongToHRDepartment")] //means the user must have a claim to satisfy MustBelongToHRDepartment policy.
                                                     //and that policy must have Department claim with HR value
    public class HumanResourceModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
