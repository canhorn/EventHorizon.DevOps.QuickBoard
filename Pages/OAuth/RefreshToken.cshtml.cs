using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using EventHorizon.DevOps.QuickBoard.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace EventHorizon.DevOps.QuickBoard.Pages.OAuth
{
    public class RefreshTokenModel
        : PageModel
    {
        private readonly AuthSettings _settings;
        private readonly HttpClient _httpClient;

        public RefreshTokenModel(
            IOptions<AuthSettings> settingsOption,
            IHttpClientFactory httpClientFactory
        )
        {
            _settings = settingsOption.Value;
            _httpClient = httpClientFactory.CreateClient();
        }

        public TokenModel Token { get; private set; }
        public string Error { get; private set; }
        public string ProfileUrl { get; private set; }

        public async Task OnGet(
            string refreshToken
        )
        {
            var error = default(string);
            if (!string.IsNullOrEmpty(
                refreshToken
            ))
            {
                // Form the request to exchange an auth code for an access token and refresh token
                var requestMessage = new HttpRequestMessage(
                    HttpMethod.Post,
                    _settings.TokenUrl
                );
                requestMessage.Headers.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(
                        "application/json"
                    )
                );

                Dictionary<string, string> form = new Dictionary<string, string>()
                {
                    { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                    { "client_assertion", _settings.Client.ClientAppSecret },
                    { "grant_type", "refresh_token" },
                    { "assertion", refreshToken },
                    { "redirect_uri", _settings.Client.CallbackUrl }
                };
                requestMessage.Content = new FormUrlEncodedContent(
                    form
                );

                // Make the request to exchange the auth code for an access token (and refresh token)
                HttpResponseMessage responseMessage = await _httpClient.SendAsync(
                    requestMessage
                );

                if (responseMessage.IsSuccessStatusCode)
                {
                    // Handle successful request
                    string body = await responseMessage.Content.ReadAsStringAsync();
                    Token = JsonSerializer.Deserialize<TokenModel>(
                        body
                    );
                }
                else
                {
                    error = responseMessage.ReasonPhrase;
                }
            }
            else
            {
                error = "Invalid refresh token";
            }

            if (string.IsNullOrEmpty(
                error
            ))
            {
                Error = error;
            }

            ProfileUrl = _settings.ProfileUrl;
        }
    }
}
