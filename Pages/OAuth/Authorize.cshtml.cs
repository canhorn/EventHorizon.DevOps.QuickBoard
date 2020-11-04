using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using EventHorizon.DevOps.QuickBoard.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace EventHorizon.DevOps.QuickBoard.Pages.OAuth
{
    public class AuthorizeModel 
        : PageModel
    {
        public static readonly Dictionary<Guid, TokenModel> AuthorizationRequests = new Dictionary<Guid, TokenModel>();

        private readonly AuthSettings _settings;

        public AuthorizeModel(
            IOptions<AuthSettings> settingsOption
        )
        {
            _settings = settingsOption.Value;
        }

        public ActionResult OnGet()
        {
            var state = Guid.NewGuid();

            AuthorizationRequests[state] = new TokenModel
            {
                IsPending = true,
            };

            return new RedirectResult(
                GetAuthorizationUrl(
                    state.ToString()
                )
            );
        }

        /// <summary>
        /// Constructs an authorization URL with the specified state value.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private string GetAuthorizationUrl(string state)
        {
            UriBuilder uriBuilder = new UriBuilder(_settings.AuthUrl);
            var queryParams = HttpUtility.ParseQueryString(uriBuilder.Query ?? string.Empty);

            queryParams["client_id"] = _settings.Client.ClientAppId;
            queryParams["response_type"] = "Assertion";
            queryParams["state"] = state;
            queryParams["scope"] = _settings.Client.Scope;
            queryParams["redirect_uri"] = _settings.Client.CallbackUrl;

            uriBuilder.Query = queryParams.ToString();

            return uriBuilder.ToString();
        }
    }
}
