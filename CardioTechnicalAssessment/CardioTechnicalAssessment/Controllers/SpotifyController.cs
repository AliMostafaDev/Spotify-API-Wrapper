using CardioTechnicalAssessment.Models;
using CardioTechnicalAssessment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CardioTechnicalAssessment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyController : ControllerBase
    {
        private readonly SpotifyAuthService _authService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public SpotifyController(SpotifyAuthService authService, HttpClient httpClient, IConfiguration Configuration)
        {
            _authService = authService;
            _httpClient = httpClient;
            this._configuration = Configuration;
        }

        [HttpGet("authenticate")]
        public IActionResult Authenticate()
        {
            string clientId = _configuration["Spotify:ClientId"];
            string redirectUri = _configuration["Spotify:RedirectUri"];
            string scope = "user-library-read playlist-modify-public playlist-read-private playlist-read-collaborative user-top-read ";  
            string state = Guid.NewGuid().ToString(); 

            string authUrl = $"https://accounts.spotify.com/authorize?client_id={clientId}&response_type=code&redirect_uri={redirectUri}&scope={scope}&state={state}";

            return Redirect(authUrl);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            try
            {
                var accessToken = await _authService.GetAccessTokenAsync(code);
                return Ok(new { AccessToken = accessToken });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("artists")]
        public async Task<IActionResult> GetArtists([FromHeader] string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me/top/artists");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var artistsResponse = JsonConvert.DeserializeObject<ArtistsResponse>(content);

                return Ok(artistsResponse.Items);
            }
            else
            {
                return BadRequest($"Error retrieving artists: {content}");
            }
        }

        [HttpGet("albums")]
        public async Task<IActionResult> GetAlbums([FromHeader] string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me/albums");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var albumsResponse = JsonConvert.DeserializeObject<AlbumsResponse>(content);

                var simplifiedAlbums = albumsResponse.Items
                    .Select(item => new Album
                    {
                        Id = item.Album.Id,
                        Name = item.Album.Name
                    }).ToList();

                return Ok(simplifiedAlbums); 
            }
            else
            {
                return BadRequest($"Error retrieving albums: {content}");
            }
        }



        [HttpGet("playlists")]
        public async Task<IActionResult> GetUserPlaylists([FromHeader] string accessToken)
        {
            var userProfileRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me");
            userProfileRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var userProfileResponse = await _httpClient.SendAsync(userProfileRequest);
            if (!userProfileResponse.IsSuccessStatusCode)
            {
                var errorContent = await userProfileResponse.Content.ReadAsStringAsync();
                return BadRequest($"Error fetching user profile: {errorContent}");
            }

            var userProfileContent = await userProfileResponse.Content.ReadAsStringAsync();
            var userProfile = JsonConvert.DeserializeObject<dynamic>(userProfileContent);
            string userId = userProfile.id;

            var playlistRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.spotify.com/v1/users/{userId}/playlists");
            playlistRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var playlistResponse = await _httpClient.SendAsync(playlistRequest);
            if (!playlistResponse.IsSuccessStatusCode)
            {
                var errorContent = await playlistResponse.Content.ReadAsStringAsync();
                return BadRequest($"Error retrieving playlists: {errorContent}");
            }

            var playlistContent = await playlistResponse.Content.ReadAsStringAsync();

            var playlists = JsonConvert.DeserializeObject<PlaylistResponse>(playlistContent);

            return Ok(playlists);
        }


        [HttpPost("playlist")]
        public async Task<IActionResult> AddToUserPlaylist([FromHeader] string accessToken, [FromBody] AddToPlaylistRequest request)
        {
            var userProfileUri = "https://api.spotify.com/v1/me";

            var profileRequest = new HttpRequestMessage(HttpMethod.Get, userProfileUri)
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", accessToken) }
            };

            var profileResponse = await _httpClient.SendAsync(profileRequest);
            var profileContent = await profileResponse.Content.ReadAsStringAsync();

            if (!profileResponse.IsSuccessStatusCode)
            {
                return BadRequest($"Error retrieving user profile: {profileContent}");
            }

            var userProfile = JsonConvert.DeserializeObject<dynamic>(profileContent);
            string userId = userProfile.id;

            var playlistUri = $"https://api.spotify.com/v1/users/{userId}/playlists";
            var body = new
            {
                name = request.Name,
                description = request.Description,
                Public = request.Public
            };

            var playlistRequest = new HttpRequestMessage(HttpMethod.Post, playlistUri)
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", accessToken) },
                Content = new StringContent(JsonConvert.SerializeObject(body), System.Text.Encoding.UTF8, "application/json")
            };

            var playlistResponse = await _httpClient.SendAsync(playlistRequest);
            var playlistContent = await playlistResponse.Content.ReadAsStringAsync();

            if (playlistResponse.IsSuccessStatusCode)
            {
                var createdPlaylist = JsonConvert.DeserializeObject(playlistContent);
                return Ok(new { Message = "Playlist created successfully" });
            }
            else
            {
                return BadRequest($"Error creating playlist: {playlistContent}");
            }
        }

    }

}
