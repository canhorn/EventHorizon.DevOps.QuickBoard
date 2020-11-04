using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace EventHorizon.DevOps.QuickBoard.Model
{
    public class TokenModel
    {
        public string Id { get; set; }
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("expires_in")]
        public string ExpiresIn { get; set; }

        public bool IsPending { get; set; }

        public void Merge(
            TokenModel token
        )
        {
            AccessToken = token.AccessToken;
            TokenType = token.TokenType;
            RefreshToken = token.RefreshToken;
            ExpiresIn = token.ExpiresIn;
        }
    }
}
