using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHorizon.DevOps.QuickBoard.Model
{
    public class AuthSettings
    {
        public string AuthUrl { get; set; }
        public string TokenUrl { get; set; }
        public string ProfileUrl { get; set; }
        public AuthClientSettings Client { get; set; }
    }

    public class AuthClientSettings
    {
        public string ClientAppId { get; set; }
        public string ClientAppSecret { get; set; }
        public string Scope { get; set; }
        public string CallbackUrl { get; set; }
    }
}
