using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.AspNetCore.Authentication.OAuth.Gitee
{
    public class GiteeAuthticationcationTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public long created_at { get; set; }
    }


    public class GiteeAuthticationcationOpenIdResponse
    {
        public string client_id { get; set; }

        public string openid { get; set; }

    }

    public class GiteeAuthticationcationUserInfoResponse
    {
        /// <summary>
        /// 也可以等于openId
        /// </summary>
        public int id { get; set; }
        public string login { get; set; }
        public string name { get; set; }
        public string avatar_url { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string remark { get; set; }
        public string followers_url { get; set; }
        public string following_url { get; set; }
        public string gists_url { get; set; }
        public string starred_url { get; set; }
        public string subscriptions_url { get; set; }
        public string organizations_url { get; set; }
        public string repos_url { get; set; }
        public string events_url { get; set; }
        public string received_events_url { get; set; }
        public string type { get; set; }
        public string blog { get; set; }
        public string weibo { get; set; }
        public string bio { get; set; }
        public int public_repos { get; set; }
        public int public_gists { get; set; }
        public int followers { get; set; }
        public int following { get; set; }
        public int stared { get; set; }
        public int watched { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string email { get; set; }
    }
}
