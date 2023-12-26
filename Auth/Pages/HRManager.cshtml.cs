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
            var httpClient = httpClientFactory.CreateClient("OurWebApi");

            var res = await httpClient.PostAsJsonAsync("auth", new Credential { UserName = "admin", Password = "password" }); // return response with jwt
            res.EnsureSuccessStatusCode(); // ensure it is succed 
            string strJwt = await res.Content.ReadAsStringAsync(); // it contains jwt with expire time
            var token = JsonConvert.DeserializeObject<JwtToken>(strJwt); // deserialize the object 

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken ?? string.Empty); //adding token to http req header which will be send along with WeatherForecast request 
            weatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast")?? new List<WeatherForecastDTO>(); //trigger the endpoint. WeatherForecast is the endpoint 
        }
    }
}
