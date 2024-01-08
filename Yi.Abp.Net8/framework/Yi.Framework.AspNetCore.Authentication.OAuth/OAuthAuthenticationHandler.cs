using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Yi.Framework.AspNetCore.Authentication.OAuth
{
    public abstract class OauthAuthenticationHandler<TOptions> : AuthenticationHandler<TOptions> where TOptions : AuthenticationSchemeOptions, new()
    {
        public abstract string AuthenticationSchemeNmae { get; }
        private AuthenticationScheme _scheme;

        public OauthAuthenticationHandler(IOptionsMonitor<TOptions> options, ILoggerFactory logger, UrlEncoder encoder, IHttpClientFactory httpClientFactory) : base(options, logger, encoder)
        {
            HttpClientFactory = httpClientFactory;
            HttpClient = HttpClientFactory.CreateClient();
        }


        protected IHttpClientFactory HttpClientFactory { get; }

        protected HttpClient HttpClient { get; }



        /// <summary>
        /// 生成认证票据
        /// </summary>
        /// <returns></returns>
        private AuthenticationTicket TicketConver(List<Claim> claims)
        {
            var claimsIdentity = new ClaimsIdentity(claims.ToArray(), AuthenticationSchemeNmae);
            var principal = new ClaimsPrincipal(claimsIdentity);
            return new AuthenticationTicket(principal, AuthenticationSchemeNmae);
        }

        protected async Task<HttpModel> SendHttpRequestAsync<HttpModel>(string url, IEnumerable<KeyValuePair<string, string?>> query, HttpMethod? httpMethod = null)
        {
            httpMethod = httpMethod ?? HttpMethod.Get;

            var queryUrl = QueryHelpers.AddQueryString(url, query);
            HttpResponseMessage response = null;
            if (httpMethod == HttpMethod.Get)
            {
                response = await HttpClient.GetAsync(queryUrl);
            }
            else if (httpMethod == HttpMethod.Post)
            {
                response = await HttpClient.PostAsync(queryUrl, null);
            }

            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"授权服务器请求错误,请求地址:{queryUrl},错误信息：{content}");
            }
            VerifyErrResponse(content);
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<HttpModel>(content);
            return model!;
        }

        protected virtual void VerifyErrResponse(string content)
        {
            AuthticationErrCodeModel.VerifyErrResponse(content);
        }

        protected abstract Task<List<Claim>> GetAuthTicketAsync(string code);


        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Context.Request.Query.ContainsKey("code"))
            {
                return AuthenticateResult.Fail("回调未包含code参数");
            }
            var code = Context.Request.Query["code"].ToString();

            List<Claim> authTicket = null;
            try
            {
                authTicket = await GetAuthTicketAsync(code);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex.Message ?? "未知错误");
            }
            //成功
            var result = AuthenticateResult.Success(TicketConver(authTicket));
            return result;
        }
    }
}


