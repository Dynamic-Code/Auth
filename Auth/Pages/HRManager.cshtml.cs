using Auth.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            weatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast")?? new List<WeatherForecastDTO>(); //trigger the endpoint. WeatherForecast is the endpoint 
        }
    }
}
