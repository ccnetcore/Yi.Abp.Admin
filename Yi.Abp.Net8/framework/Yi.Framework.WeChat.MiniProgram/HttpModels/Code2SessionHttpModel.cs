using Yi.Framework.WeChat.MiniProgram.Abstract;

namespace Yi.Framework.WeChat.MiniProgram.HttpModels;


    public class Code2SessionResponse: IErrorObjct
    {
        public string openid { get; set; }
        public string session_key { get; set; }
        public string unionid { get; set; }
        public int errcode { get; set; }
        public string errmsg { get; set; }
    }

    public class Code2SessionRequest
    {
        public string appid { get; set; }
        public string secret { get; set; }
        public string js_code { get; set; }
        public string grant_type => "authorization_code";
    }

    public class Code2SessionInput
    {
        public Code2SessionInput(string js_code)
        { 
        
            this.js_code=js_code;
        }
        public string js_code { get; set; }
    }
