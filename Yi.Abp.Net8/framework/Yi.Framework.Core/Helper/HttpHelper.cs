using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yi.Framework.Core.Helper
{
    public static class HttpHelper
    {

        public static HttpClient Client { get; set; } = new HttpClient();

        public static async Task<string> Get(string url)
        {
            return await Client.GetStringAsync(url);
        }

        public static async Task<Stream> GetIO(string url)
        {
            return await Client.GetStreamAsync(url);
        }


        public static async Task<string> Post(string url, object? item = null, Dictionary<string, string>? head = null)
        {

            using StringContent json = new(JsonSerializer.Serialize(item), Encoding.UTF8, MediaTypeNames.Application.Json);


            if (head is not null)
            {
                foreach (var d in head)
                {
                    json.Headers.Add(d.Key, d.Value);
                }
            }

            var httpResponse = await Client.PostAsync(url, json);

            httpResponse.EnsureSuccessStatusCode();

            var content = httpResponse.Content;

            return await content.ReadAsStringAsync();
        }


        //        public static string HttpGet(string Url, string postDataStr="")
        //        {
        //#pragma warning disable SYSLIB0014 // 类型或成员已过时
        //            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
        //#pragma warning restore SYSLIB0014 // 类型或成员已过时
        //            request.Method = "GET";
        //            request.ContentType = "text/html;charset=UTF-8";

        //            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //            Stream myResponseStream = response.GetResponseStream();
        //            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
        //            string retString = myStreamReader.ReadToEnd();
        //            myStreamReader.Close();
        //            myResponseStream.Close();

        //            return retString;
        //        }

        //        public static bool HttpIOGet(string Url, string file, string postDataStr="")
        //        {
        //            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
        //            request.Method = "GET";
        //            request.ContentType = "text/html;charset=UTF-8";

        //            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //            Stream myResponseStream = response.GetResponseStream();
        //            FileStream writer = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write);
        //            byte[] buffer = new byte[1024];
        //            int c;
        //            while ((c = myResponseStream.Read(buffer, 0, buffer.Length)) > 0)
        //            {
        //                writer.Write(buffer, 0, c);
        //            }
        //            writer.Close();
        //            myResponseStream.Close();

        //            return true;
        //        }

        //        public static string HttpPost(string Url, string postDataStr="")
        //        {
        //            CookieContainer cookie = new CookieContainer();
        //#pragma warning disable SYSLIB0014 // 类型或成员已过时
        //            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
        //#pragma warning restore SYSLIB0014 // 类型或成员已过时
        //            request.Method = "POST";
        //            request.ContentType = "application/x-www-form-urlencoded";
        //            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
        //            request.CookieContainer = cookie;

        //            Stream myRequestStream = request.GetRequestStream();
        //            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
        //            myStreamWriter.Write(postDataStr);
        //            myStreamWriter.Close();

        //            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        //            response.Cookies = cookie.GetCookies(response.ResponseUri);
        //            Stream myResponseStream = response.GetResponseStream();
        //            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
        //            string retString = myStreamReader.ReadToEnd();
        //            myStreamReader.Close();
        //            myResponseStream.Close();

        //            return retString;
        //        }
    }
}
