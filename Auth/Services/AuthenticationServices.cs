using EventHorizon.DevOps.QuickBoard.Auth.Model;
using EventHorizon.DevOps.QuickBoard.Model;
using EventHorizon.DevOps.QuickBoard.Pages.OAuth;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventHorizon.DevOps.QuickBoard.Auth.Services
{
    public interface AuthenticationServices
    {
        public AuthenticationState State { get; }

        public Task Init();

        public Task SetTokenDetails(TokenModel tokenDetails);

        public void OnChanged(Func<Task> onChanged);
    }

    public class StandardAuthenticationServices
        : AuthenticationServices
    {
        private readonly IList<Func<Task>> OnChangedList = new List<Func<Task>>();

        private readonly IJSRuntime _runtime;

        public AuthenticationState State { get; private set; }

        public StandardAuthenticationServices(
            IJSRuntime runtime
        )
        {
            _runtime = runtime;

            State = new AuthenticationState(
                false,
                new TokenModel
                {
                    IsPending = true,
                }
            );
        }

        public async Task Init()
        {
            var tokenId = await _runtime.InvokeAsync<string>(
                "Interop.getCookie",
                "ehz-auth_id"
            );
            // Check for not existing auth id
            if (string.IsNullOrWhiteSpace(tokenId))
            {
                // We just want to ignore this, since the user is not authenticated anymore
                return;
            }

            var tokenDetails = await _runtime.InvokeAsync<TokenModel>(
                "Interop.getTokenDetails"
            );
            if (!string.IsNullOrWhiteSpace(tokenDetails.AccessToken)
                && tokenId == tokenDetails.Id
            )
            {
                // We have token access details
                State = new AuthenticationState(
                    true,
                    tokenDetails
                );
                foreach (var onChanged in OnChangedList)
                {
                    await onChanged();
                }

                return;
            }

            // Check for existing TokenDetails in Memory
            if(AuthorizeModel.AuthorizationRequests.TryGetValue(
                new Guid(tokenId),
                out var token
            ))
            {
                await SetTokenDetails(
                    token
                );
            }

        }

        public void OnChanged(
            Func<Task> onChanged
        )
        {
            OnChangedList.Add(
                onChanged
            );
        }

        public async Task SetTokenDetails(
            TokenModel tokenDetails
        )
        {
            if (!string.IsNullOrWhiteSpace(tokenDetails.AccessToken))
            {
                await _runtime.InvokeVoidAsync(
                    "Interop.setTokenDetails",
                    tokenDetails
                );
                State = new AuthenticationState(
                    true,
                    tokenDetails
                );
                foreach (var onChanged in OnChangedList)
                {
                    await onChanged();
                }
            }

        }
    }
}
