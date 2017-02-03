using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace xiaoiceuwp
{
    public class MsgManager
    {
        private static string ans = "can not retrieve message";

        public static async Task PostMsg(SinaCookie cookie, long uid, string msg = "happy new year")
        {
            string url = "http://weibo.com/aj/message/add";
            string weibourl = "weibo.com";
            Uri weibouri = new Uri("http://weibo.com");
            try
            {

                var t = (DateTime.Now.Ticks - 621355968000000000) / 10000000;

                string data = "ajwvr=6&__rnd=" + t + "&location=msgdialog&module=msgissue&style_id=1&text=" + msg + $"&uid={uid}&tovfids=&fids=&el=[object HTMLDivElement]&_t=0";
                CookieContainer cookieContainer = new CookieContainer();
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
                //设置cookies
                cookieContainer.Add(weibouri, new Cookie(nameof(cookie.wvr), cookie.wvr, "/", weibourl));
                cookieContainer.Add(weibouri, new Cookie(nameof(cookie.un), cookie.un, "/", weibourl));
                cookieContainer.Add(weibouri, new Cookie(nameof(cookie.SUB), cookie.SUB, "/", weibourl));
                cookieContainer.Add(weibouri, new Cookie(nameof(cookie.SUBP), cookie.SUBP, "/", weibourl));
                cookieContainer.Add(weibouri, new Cookie(nameof(cookie.SUHB), cookie.SUHB, "/", weibourl));
                cookieContainer.Add(weibouri, new Cookie(nameof(cookie.ALF), cookie.ALF, "/", weibourl));
                cookieContainer.Add(weibouri, new Cookie(nameof(cookie.SSOLoginState), cookie.SSOLoginState, "/", weibourl));
                cookieContainer.Add(weibouri, new Cookie(nameof(cookie._s_tentry), cookie._s_tentry, "/", weibourl));
                cookieContainer.Add(weibouri, new Cookie(nameof(cookie.UOR), cookie.UOR, "/", weibourl));
                var c = hc.SendAsync(request);
                Task<string> respond = c.Result.Content.ReadAsStringAsync();
                JObject result = JObject.Parse(respond.Result);
                if (!result["code"].ToString().Equals("100000"))
                {
                    await new MessageDialog(result["msg"].ToString()).ShowAsync();
                }

            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
        }
        private static async Task<bool> TryGetMsg(SinaCookie cookie, long uid)
        {
            string uri = "http://m.weibo.cn/msg/messages?uid=5175429989&page=1";
            try
            {
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
                request.Headers.TransferEncodingChunked = false;
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler()
                {
                    UseCookies = false
                };
                request.Headers.Add("Cookie", $"SUB={cookie.SUB};wvr={cookie.wvr};UOR={cookie.UOR};_s_tentry={cookie._s_tentry};SSOLoginState={cookie.SSOLoginState};SUHB={cookie.SUHB};SUBP={cookie.SUBP};un={cookie.un};ALF={cookie.ALF};");
                HttpClient hc = new HttpClient(handler);
                var re = await hc.SendAsync(request);
                Debug.WriteLine(re.StatusCode);
                //2017年2月3日测试
                var b = re.Content.Headers.ContentEncoding;
                var st =await re.Content.ReadAsStreamAsync();
                StreamReader sr = new StreamReader(st, Encoding.ASCII);
                var strr = re.Content.ReadAsStringAsync().Result;
                string respond = sr.ReadToEnd();
                //var respond = re.Content.ReadAsStringAsync();
                if (respond != null)
                {
                    await new MessageDialog(respond).ShowAsync();
                    string txt = respond;
                    JObject result = JObject.Parse(txt);
                    Debug.WriteLine(result["msg"]);
                    if (result["ok"].ToString().Equals("1"))
                    {
                        var msg = JsonConvert.DeserializeObject<Respond>(txt);
                        if (msg.data[0].sender_id == uid)
                        {
                            Debug.WriteLine(msg.data[0].text);
                            ans = msg.data[0].text;
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
                return false;
            }
        }

        public static async Task<string> GetMsg(SinaCookie cookie, long uid)
        {
            int retrycount = 5;
            bool getresult = false;
            while (!getresult && (retrycount-- > 0))
            {
                await Task.Delay(1024);
                getresult = await TryGetMsg(cookie, uid);
            }
            return ans;
        }

    }
}
