using Auth.Authorization;
using Auth.DTO;
using Auth.Pages.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Auth.Pages
{
    [Authorize(Policy = "HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        // want to display the weather forecast once the HRManager is login 
        private readonly IHttpClientFactory httpClientFactory;

        [BindProperty]
        public List<WeatherForecastDTO> weatherForecastItems { get; set; } = new List<WeatherForecastDTO>(); //created this prop to bind and display weatherforecast data
        public HRManagerModel(IHttpClientFactory httpClientFactory) // DI HttpClientFacotry to trigger the web api endpoint
        {
            this.httpClientFactory = httpClientFactory;
        }
        public async Task OnGetAsync()
        {
            //Get token form sesion
            JwtToken token = new JwtToken();

            var strTokenObj = HttpContext.Session.GetString("access_token"); // getting token from session
            if (strTokenObj == null) // means we dont have token in session so get the token and authenticate with below logic
            {
               token = await Authenticate(); // get the token and authenticate
            }
            else // means we have the token in session so deseralize it 
            {
                token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj) ?? new JwtToken();
            }

            if (token == null || 
                string.IsNullOrWhiteSpace(token.AccessToken) || 
                token.ExpiresAt <= DateTime.UtcNow) // checking the possible token fail scenarios
            {
                token = await Authenticate();
            }
            //after getting the token from above operations
            var httpClient = httpClientFactory.CreateClient("OurWebApi"); //creating client
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken ?? string.Empty); //adding token to http req header which will be send along with WeatherForecast request 
            weatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast")?? new List<WeatherForecastDTO>(); //trigger the endpoint. WeatherForecast is the endpoint 
        }
        private async Task<JwtToken> Authenticate()
        {
            // Authenticating and getting token
            var httpClient = httpClientFactory.CreateClient("OurWebApi"); //creating client
            var res = await httpClient.PostAsJsonAsync("auth", new Credential { UserName = "admin", Password = "password" }); // return response with jwt
            res.EnsureSuccessStatusCode(); // ensure it is succeded 
            string strJwt = await res.Content.ReadAsStringAsync(); // it contains jwt with expire time

            HttpContext.Session.SetString("access_token", strJwt); // ADDING SESSION IN OUR SESSION

            return JsonConvert.DeserializeObject<JwtToken>(strJwt) ?? new JwtToken(); // deserialize the object 
        }
    }
}
