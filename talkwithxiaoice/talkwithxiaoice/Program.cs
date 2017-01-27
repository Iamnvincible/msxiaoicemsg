using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace talkwithxiaoice
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                int count = 0;
                Console.WriteLine("say something to ms-xiaoice");
                string msg = Console.ReadLine();
                PostMsg(msg);
                while (!GetMsg() && count < 5)
                {
                    Thread.Sleep(2000);
                    count++;
                }
                if (count >= 5)
                {
                    Console.WriteLine("她没回复...");
                }
            }

        }


        static void PostMsg(string msg = "happy new year")
        {
            string url = "http://weibo.com/aj/message/add";
            var t = (DateTime.Now.Ticks - 621355968000000000) / 10000000;

            string data = "ajwvr=6&__rnd=" + t + "&location=msgdialog&module=msgissue&style_id=1&text=" + msg + "&uid=5175429989&tovfids=&fids=&el=[object HTMLDivElement]&_t=0";
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer
            };
            HttpClient hc = new HttpClient(handler);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

            //设置请求头
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8,ko;q=0.6,en;q=0.4,zh-TW;q=0.2,fr;q=0.2");
            request.Headers.Add("Connection", "keep-alive");
            request.Content = new StringContent(data);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            request.Headers.Add("Host", "weibo.com");
            request.Headers.Add("Origin", "http://weibo.com");
            request.Headers.Add("Referer", "http://weibo.com/message/history?uid=5175429989&name=%E5%B0%8F%E5%86%B0");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.143 Safari/537.36");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");

            ChromeCookieReader r = new ChromeCookieReader();
            var cc = r.ReadCookies();
            var list = cc.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Item1.Equals("UOR"))
                {
                    cookieContainer.Add(new Cookie(list[i].Item1.ToString(), "login.sina.com.cn", "/", "weibo.com"));
                    continue;
                }
                cookieContainer.Add(new Cookie(list[i].Item1.ToString(), list[i].Item2.ToString().Replace(',', ' '), "/", "weibo.com"));
            }

            var c = hc.SendAsync(request);
            Task<string> respond = c.Result.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(respond.Result);

            Console.WriteLine(result["code"] + "\n" + result["msg"]);
        }

        static bool GetMsg()
        {
            string uri = "http://m.weibo.cn/msg/messages?uid=5175429989&page=1";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("Host", "m.weibo.cn");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Cache-Control", "max-age=0");
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Headers.Add("Referer", "http://m.weibo.cn/msg/chat?uid=5175429989");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1");
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8,ko;q=0.6,en;q=0.4,zh-TW;q=0.2,fr;q=0.2");
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler()
            {
                UseCookies = false
            };
            StringBuilder cookiecotent = new StringBuilder();
            ChromeCookieReader r = new ChromeCookieReader();
            var cc = r.ReadCookies();
            var list = cc.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Item1.Equals("UOR"))
                {
                    cookiecotent.Append(list[i].Item1 + "=" + "login.sina.com.cn" + ";");
                    continue;
                }
                cookiecotent.Append(list[i].Item1 + "=" + list[i].Item2 + ";");
            }
            request.Headers.Add("Cookie", cookiecotent.ToString());
            HttpClient hc = new HttpClient(handler);
            var re = hc.SendAsync(request);
            Console.WriteLine(re.Result.StatusCode);
            var respond = re.Result.Content.ReadAsStringAsync();
            if (respond != null)
            {
                string txt = respond.Result;
                JObject result = JObject.Parse(txt);
                Console.WriteLine(result["msg"]);
                if (result["ok"].ToString().Equals("1"))
                {
                    var msg = JsonConvert.DeserializeObject<Respond>(txt);
                    if (msg.data[0].sender_id == 5175429989)
                    {
                        Console.WriteLine(msg.data[0].text);
                        return true;
                    }
                }
            }
            return false;

        }

    }
}
