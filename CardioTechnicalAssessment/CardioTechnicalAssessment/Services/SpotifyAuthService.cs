using CardioTechnicalAssessment.Models;
using Newtonsoft.Json;

namespace CardioTechnicalAssessment.Services
{
    public class SpotifyAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public SpotifyAuthService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<string> GetAccessTokenAsync(string authorizationCode)
        {
            var clientId = _configuration["Spotify:ClientId"];
            var clientSecret = _configuration["Spotify:ClientSecret"];
            var redirectUri = _configuration["Spotify:RedirectUri"];

            var requestBody = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", authorizationCode),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret)
        });

            var response = await _httpClient.PostAsync("https://accounts.spotify.com/api/token", requestBody);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = JsonConvert.DeserializeObject<SpotifyTokenResponse>(responseContent);
                return tokenResponse.AccessToken;
            }
            else
            {
                throw new Exception("Error exchanging authorization code for token.");
            }
        }
    }
}
