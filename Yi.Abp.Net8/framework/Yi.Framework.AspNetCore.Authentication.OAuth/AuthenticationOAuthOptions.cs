using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Yi.Framework.AspNetCore.Authentication.OAuth
{
    public class AuthenticationOAuthOptions:OAuthOptions
    {

        public string RedirectUri { get; set; }
    }
}
