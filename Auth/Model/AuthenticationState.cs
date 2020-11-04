using EventHorizon.DevOps.QuickBoard.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHorizon.DevOps.QuickBoard.Auth.Model
{
    public class AuthenticationState
    {
        public bool IsAuthenticated { get; }
        public TokenModel TokenDetails { get; }

        public AuthenticationState(
            bool isAuthenticated,
            TokenModel tokenDetails
        )
        {
            IsAuthenticated = isAuthenticated;
            TokenDetails = tokenDetails;
        }
    }
}
