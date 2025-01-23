﻿using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace CardioTechnicalAssessment.Models
{
    public class SpotifyTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }

    }
}