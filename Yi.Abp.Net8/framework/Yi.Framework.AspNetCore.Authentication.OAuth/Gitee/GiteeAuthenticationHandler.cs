using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Yi.Framework.AspNetCore.Authentication.OAuth.Gitee.GiteeAuthenticationConstants;

namespace Yi.Framework.AspNetCore.Authentication.OAuth.Gitee
{
    public class GiteeAuthenticationHandler : OauthAuthenticationHandler<GiteeAuthenticationOptions>
    {
        public GiteeAuthenticationHandler(IOptionsMonitor<GiteeAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, IHttpClientFactory httpClientFactory) : base(options, logger, encoder, httpClientFactory)
        {
        }

        public override string AuthenticationSchemeNmae => GiteeAuthenticationDefaults.AuthenticationScheme;

        protected override async Task<List<Claim>> GetAuthTicketAsync(string code)
        {
            //获取 accessToken
            var tokenQueryKv = new List<KeyValuePair<string, string?>>()
            {
                new KeyValuePair<string, string?>("grant_type","authorization_code"),
                new KeyValuePair<string, string?>("client_id",Options.ClientId),
                new KeyValuePair<string, string?>("client_secret",Options.ClientSecret),
                new KeyValuePair<string, string?>("redirect_uri",Options.RedirectUri),
                new KeyValuePair<string, string?>("code",code)
            };
            var tokenModel = await SendHttpRequestAsync<GiteeAuthticationcationTokenResponse>(GiteeAuthenticationDefaults.TokenEndpoint, tokenQueryKv,HttpMethod.Post);

            //获取 userInfo
            var userInfoQueryKv = new List<KeyValuePair<string, string?>>()
            {
                new KeyValuePair<string, string?>("access_token",tokenModel.access_token),
            };
            var userInfoMdoel = await SendHttpRequestAsync<GiteeAuthticationcationUserInfoResponse>(GiteeAuthenticationDefaults.UserInformationEndpoint, userInfoQueryKv);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(Claims.AvatarUrl, userInfoMdoel.avatar_url),
                new Claim(Claims.Url, userInfoMdoel.url),

                new Claim(AuthenticationConstants.OpenId,userInfoMdoel.id.ToString()),
                new Claim(AuthenticationConstants.Name, userInfoMdoel.name),
                new Claim(AuthenticationConstants.AccessToken, tokenModel.access_token)
            };
            return claims;
        }
    }
}
