using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WEB_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }
        [HttpPost]
        public IActionResult Authenticate([FromBody] Credential credential)
        {
            // verfiy the cred
            if (credential.UserName == "admin" && credential.Password == "password")
            {
                // creating the security Context for JWT
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

                var expiryAt = DateTime.UtcNow.AddMinutes(10); //Added expiry time

                return Ok(new
                {
                    access_token = CreateToken(claims,expiryAt),  // it is empty here now. We will create a pvt method to get the token
                    expires_at = expiryAt // It is for client 
                });
            }
            ModelState.AddModelError("Unauthorized", "You are not authorized to access the endpoint");
            return Unauthorized(ModelState);
        }

        private string CreateToken(IEnumerable<Claim> claims, DateTime expireAt) // method to generate access token
        {
            var secretKey = Encoding.ASCII.GetBytes(_config.GetValue<string>("secretKey")??""); // getting secretKey in Bytes
            // generate JWT
            // install nuget package
            var jwt = new JwtSecurityToken(   // creating JWT object
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expireAt,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature));

            return new JwtSecurityTokenHandler().WriteToken(jwt); // convert jwt object to string and return
        }
         
        public class Credential
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    }
}
