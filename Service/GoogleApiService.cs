using Azure;
using GoWheels_WebAPI.Models.GoogleRespone;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace GoWheels_WebAPI.Service
{
    public class GoogleApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GoogleApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<DistanceMatrixRespone> GetDistanceAsync(List<(string userId, string location)> userLocations, string  destination)
        {
            var startLocation = string.Join("|", userLocations.Select(loc => loc.location));
            var apiKey = _configuration["GoogleApi:ApiKey"];
            var url = $"json?origins={startLocation}&destinations={destination}&key={apiKey}";
            var response = await _httpClient.GetAsync(url);
            var responeContent = await response.Content.ReadAsStringAsync();
            if(responeContent.IsNullOrEmpty())
            {
                throw new NullReferenceException("Google respone is null");
            }    
            var distanceMatrixRespone = JsonSerializer.Deserialize<DistanceMatrixRespone>(responeContent);
            if(distanceMatrixRespone == null)
            {
                throw new NullReferenceException("Distance matrix respone is null");
            }    
            return distanceMatrixRespone;
        }
    }
}
