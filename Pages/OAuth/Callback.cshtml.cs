using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using EventHorizon.DevOps.QuickBoard.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace EventHorizon.DevOps.QuickBoard.Pages.OAuth
{
    public class CallbackModel
        : PageModel
    {
        private readonly AuthSettings _settings;
        private readonly HttpClient _httpClient;

        public CallbackModel(
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
            string code,
            Guid state
        )
        {
            var error = string.Empty;
            if (ValidateCallbackValues(code, state.ToString(), out error))
            {
                // Exchange the auth code for an access token and refresh token
                HttpRequestMessage requestMessage = new HttpRequestMessage(
                    HttpMethod.Post,
                    _settings.TokenUrl
                );
                requestMessage.Headers.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json")
                );

                Dictionary<string, string> form = new Dictionary<string, string>()
                {
                    { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                    { "client_assertion", _settings.Client.ClientAppSecret },
                    { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer" },
                    { "assertion", code },
                    { "redirect_uri", _settings.Client.CallbackUrl }
                };
                requestMessage.Content = new FormUrlEncodedContent(
                    form
                );

                HttpResponseMessage responseMessage = await _httpClient.SendAsync(
                    requestMessage
                );

                if (responseMessage.IsSuccessStatusCode)
                {
                    var body = await responseMessage.Content.ReadAsStringAsync();

                    var tokenModel = AuthorizeModel.AuthorizationRequests[state];
                    var bodyToken = JsonSerializer.Deserialize<TokenModel>(
                        body
                    );
                    tokenModel.Merge(
                        bodyToken
                    );

                    Token = tokenModel;
                    Token.Id = state.ToString();

                    Response.Cookies.Append(
                        "ehz-auth_token",
                        Token.AccessToken,
                        new CookieOptions
                        {
                            MaxAge = TimeSpan.FromSeconds(
                                int.Parse(Token.ExpiresIn)
                            )
                        }
                    );
                    Response.Cookies.Append(
                        "ehz-auth_id",
                        state.ToString(),
                        new CookieOptions
                        {
                            MaxAge = TimeSpan.FromSeconds(
                                int.Parse(Token.ExpiresIn)
                            )
                        }
                    );
                }
                else
                {
                    error = responseMessage.ReasonPhrase;
                }
            }

            if (!string.IsNullOrEmpty(error))
            {
                Error = error;
            }

            ProfileUrl = _settings.ProfileUrl;
        }

        private bool ValidateCallbackValues(
            string code,
            string state,
            out string error)
        {
            error = null;

            if (string.IsNullOrEmpty(code))
            {
                error = "Invalid auth code";
            }
            else
            {
                if (Guid.TryParse(
                    state,
                    out var authorizationRequestKey
                ))
                {
                    if (!AuthorizeModel.AuthorizationRequests.TryGetValue(
                        authorizationRequestKey,
                        out var tokenModel
                    ))
                    {
                        error = "Unknown authorization request key";
                    }
                    else if (!tokenModel.IsPending)
                    {
                        error = "Authorization request key already used";
                    }
                    else
                    {
                        AuthorizeModel
                            .AuthorizationRequests[authorizationRequestKey]
                            .IsPending = false; // mark the state value as used so it can't be reused
                    }
                }
                else
                {
                    error = "Invalid authorization request key";
                }
            }

            return error == null;
        }
    }
}
