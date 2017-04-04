using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace xiaoiceuwp
{

    public class Mweibologin
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public Mweibologin(string u, string p)
        {
            this.UserName = u;
            this.PassWord = p;
        }
        public async Task<string> LoginAsync()
        {
            string loginurl = "https://passport.weibo.cn/sso/login";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", UserName),
                new KeyValuePair<string, string>("password", PassWord),
                new KeyValuePair<string, string>("savestate", "1"),
                new KeyValuePair<string, string>("ec", "0"),
                new KeyValuePair<string, string>("pagerefer", "https%3A%2F%2Fpassport.weibo.cn%2Fsignin%2Fwelcome%3Fentry%3Dmweibo%26r%3Dhttp%253A%252F%252Fm.weibo.cn%252F%26wm%3D3349%26vt%3D4"),
                new KeyValuePair<string, string>("entry", "mweibo"),
                new KeyValuePair<string, string>("wentry", ""),
                new KeyValuePair<string, string>("loginfrom", ""),
                new KeyValuePair<string, string>("client_id", ""),
                new KeyValuePair<string, string>("code", ""),
                new KeyValuePair<string, string>("qq", ""),
                new KeyValuePair<string, string>("hff", ""),
                new KeyValuePair<string, string>("hfp", ""),
            });
            CookieContainer cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            HttpClient hc = new HttpClient(handler);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, loginurl);

            //设置请求头
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
            request.Headers.Add("Connection", "keep-alive");
            request.Content = content;
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            request.Headers.Add("Host", "passport.weibo.cn");
            request.Headers.Add("Origin", $"https://passport.weibo.cn");
            request.Headers.Add("Referer", $"https://passport.weibo.cn/signin/login?entry=mweibo&res=wel&wm=3349&r=http%3A%2F%2Fm.weibo.cn%2F");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.111 Safari/537.36");
            var respond = await hc.SendAsync(request);
            var cookies = respond.Headers.GetValues("Set-Cookie");
            foreach (var cookie in cookies)
            {
                if (cookie.StartsWith("SUB"))
                {
                    MainPage.sinacookie.SUB = cookie.Substring(4, 86);
                    continue;
                }
                if (cookie.StartsWith("SUHB"))
                {
                    MainPage.sinacookie.SUHB = cookie.Substring(5, 14);
                    continue;
                }
                if (cookie.StartsWith("SSOLoginState"))
                {
                    MainPage.sinacookie.SSOLoginState = cookie.Substring(14, 10);
                    continue;
                }
                if (cookie.StartsWith("ALF"))
                {
                    MainPage.sinacookie.ALF = cookie.Substring(4, 10);
                }
            }


            var text = await respond.Content.ReadAsStringAsync();

            //string text= "{\"retcode\":20000000,\"msg\":\"\",\"data\":{\"crossdomainlist\":{\"weibo.com\":\"\\/\\/passport.weibo.com\\/sso\\/crossdomain?entry=mweibo&action=login&proj=1&ticket=ST-MTkzODk4ODg5NQ%3D%3D-1491283361-gz-8095D4C42C35FBB671CCDF75AEABA71B-1\",\"sina.com.cn\":\"\\/\\/login.sina.com.cn\\/sso\\/crossdomain?entry=mweibo&action=login&proj=1&ticket=ST-MTkzODk4ODg5NQ%3D%3D-1491283361-gz-1C16E6D23FAB131F69E7534175EFDD14-1\",\"weibo.cn\":\"\\/\\/passport.sina.cn\\/sso\\/crossdomain?entry=mweibo&action=login&ticket=ST-MTkzODk4ODg5NQ%3D%3D-1491283361-gz-17E2A5C44547594F055FF87580B5EE31-1\"},\"loginresulturl\":\"\",\"uid\":\"1938988895\"}}";
            JObject jo = JObject.Parse(text);
            var count = jo["data"]["crossdomainlist"].Values();
            foreach (var item in count)
            {
                string get = "http:" + item.ToString();
                var rr = hc.GetAsync(get);
            }
            var zr = await hc.GetAsync("http://m.weibo.cn");
            var subp = cookieContainer.GetCookies(new Uri("http://sina.cn"))["SUBP"].Value;
            MainPage.sinacookie.SUBP = subp;
            return ((int)(zr.StatusCode)).ToString();

        }
    }
}
