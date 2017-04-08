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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

namespace xiaoiceuwp
{
    public class MsgManager
    {
        private static SimpleRespond sr;
        public static async Task<string> GetSt(SinaCookie sinacookie)
        {
            string url = "http://m.weibo.cn/msg/chat?uid={xiaoiceid}";
            string weibourl = ".weibo.cn";
            Uri weibouri = new Uri("http://m.weibo.cn");
            CookieContainer cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            HttpClient hc = new HttpClient(handler);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

            //先用已有的cookie对url进行一次访问，对响应中的html做一个解析找到需要的st,可参考下面的PostMsg方法
            //设置请求头

            //设置cookies

            //发送请求

            //获取响应内容 即html内容

            //用你的方法从html字符串中获取st的值
            string st = "从html中找到我";
            return st;

        }

        public static async Task<bool> PostMsg(SinaCookie cookie, long uid, string msg = "happy new year")
        {
            string url = "http://m.weibo.cn/msgDeal/sendMsg?";
            string weibourl = ".weibo.cn";
            Uri weibouri = new Uri("http://m.weibo.cn");
            string st = await GetSt(cookie);
            try
            {
                string data = "fileId\":\"null\"" + $"&uid={uid}&content={msg}&st={st}";
                CookieContainer cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler()
                {
                    CookieContainer = cookieContainer,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                HttpClient hc = new HttpClient(handler);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

                //设置请求头
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.Headers.Add("Accept-Language", "zh-Hans-CN,zh-Hans;q=0.8,en-US;q=0.5,en;q=0.3");
                request.Headers.Add("Connection", "keep-alive");
                request.Content = new StringContent(data);
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                request.Headers.Add("Host", "m.weibo.cn");
                request.Headers.Add("DNT", "1");
                request.Headers.Add("Origin", "http://m.weibo.cn");
                request.Headers.Add("Referer", $"http://m.weibo.cn/msg/chat?uid={MainPage.xiaoiceid}");
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.15063");
                request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                //设置cookies
                cookieContainer.Add(weibouri, new Cookie(nameof(cookie.SUB), cookie.SUB, "/", weibourl));
                cookieContainer.Add(weibouri, new Cookie(nameof(cookie.SUBP), cookie.SUBP, "/", weibourl));
                cookieContainer.Add(weibouri, new Cookie(nameof(cookie.SUHB), cookie.SUHB, "/", weibourl));
                cookieContainer.Add(weibouri, new Cookie(nameof(cookie.ALF), cookie.ALF, "/", weibourl));
                cookieContainer.Add(weibouri, new Cookie(nameof(cookie.SSOLoginState), cookie.SSOLoginState, "/", weibourl));
                var c = hc.SendAsync(request);
                string respond = await c.Result.Content.ReadAsStringAsync();
                JObject result = JObject.Parse(respond);
                if (!result["ok"].ToString().Equals("1"))
                {
                    await new MessageDialog(result["ok"].ToString() + result["msg"].ToString()).ShowAsync();
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
                return false;
            }
        }
        private static async Task<bool> TryGetMsg(SinaCookie cookie, long uid)
        {
            string uri = $"http://m.weibo.cn/msg/messages?uid={MainPage.xiaoiceid}&page=1";
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.Add("Host", "m.weibo.cn");
                request.Headers.Add("Connection", "keep-alive");
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.15063");
                request.Headers.Add("Accept", "text/html, application/xhtml+xml, image/jxr, */*");
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.Headers.Add("Accept-Language", "zh-Hans-CN, zh-Hans; q=0.8, en-US; q=0.5, en; q=0.3");
                request.Headers.Add("DNT", "1");
                request.Headers.Add("Pragma", "no-cache");
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler()
                {
                    UseCookies = false,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                request.Headers.Add("Cookie", $"SUB={cookie.SUB};");
                HttpClient hc = new HttpClient(handler);
                var re = await hc.SendAsync(request);
                Debug.WriteLine(re.StatusCode);
                var respond = await re.Content.ReadAsStringAsync();
                if (respond != null)
                {
                    string txt = respond;
                    JObject result = JObject.Parse(txt);
                    Debug.WriteLine(result["msg"]);
                    if (result["ok"].ToString().Equals("1"))
                    {
                        var msg = JsonConvert.DeserializeObject<Respond>(txt);
                        if (msg.data[0].sender_id == uid)
                        {
                            if (sr == null)
                            {
                                sr = new SimpleRespond();
                            }
                            sr.Message = msg.data[0].text;
                            if (msg.data[0].text == "分享语音")
                            {

                                sr.Voice = msg.data[0].attachment[0].fid.ToString();
                                //下载歌曲
                                await DownloadVoice(hc, cookie, sr.Voice);
                            }
                            if (msg.data[0].text == "分享图片")
                            {

                                sr.Image = msg.data[0].attachment[0].thumbnail_600.ToString();
                                await DownloadImg(hc, cookie, sr.Image);
                            }
                            Debug.WriteLine(msg.data[0].text);
                            sr.Message = msg.data[0].text;
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

        public static async Task<SimpleRespond> GetMsg(SinaCookie cookie, long uid)
        {
            //尝试三次获取回复，每次间隔1秒
            int retrycount = 3;
            bool getresult = false;
            while (!getresult && (retrycount-- > 0))
            {
                await Task.Delay(1024);
                getresult = await TryGetMsg(cookie, uid);
            }
            return sr;
        }
        private static async Task DownloadVoice(HttpClient hc, SinaCookie cookie, string url)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Host", "upload.api.weibo.com");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Cache-Control", "max-age=0");
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            request.Headers.Add("DNT", "1");
            request.Headers.Add("Referer", $"http://weibo.com/message/history?uid={MainPage.xiaoiceid}");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36 Edge/14.14393");
            request.Headers.Add("Accept", "text/html, application/xhtml+xml, image/jxr, */*");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "zh-Hans-CN,zh-Hans;q=0.8,en-US;q=0.5,en;q=0.3");
            //request.Headers.Add("Cookie", $"SUB={cookie.SUB};wvr={cookie.wvr};UOR={cookie.UOR};_s_tentry={cookie._s_tentry};SSOLoginState={cookie.SSOLoginState};SUHB={cookie.SUHB};SUBP={cookie.SUBP};un={cookie.un};ALF={cookie.ALF};");
            request.Headers.Add("Cookie", $"SUB={cookie.SUB};SSOLoginState={cookie.SSOLoginState};SUHB={cookie.SUHB};SUBP={cookie.SUBP};ALF={cookie.ALF};");

            var re = await hc.SendAsync(request);
            var respond = await re.Content.ReadAsByteArrayAsync();
            var file = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("ttsvoice.mp3", CreationCollisionOption.ReplaceExisting);
            var openstream = await file.OpenStreamForWriteAsync();
            await openstream.WriteAsync(respond, 0, respond.Length);
            openstream.Dispose();
        }
        private static async Task DownloadImg(HttpClient hc, SinaCookie cookie, string url)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Host", "upload.api.weibo.com");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Cache-Control", "max-age=0");
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            request.Headers.Add("DNT", "1");
            request.Headers.Add("Referer", $"http://weibo.com/message/history?uid={MainPage.xiaoiceid}");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36 Edge/14.14393");
            request.Headers.Add("Accept", "text/html, application/xhtml+xml, image/jxr, */*");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "zh-Hans-CN,zh-Hans;q=0.8,en-US;q=0.5,en;q=0.3");
            //request.Headers.Add("Cookie", $"SUB={cookie.SUB};wvr={cookie.wvr};UOR={cookie.UOR};_s_tentry={cookie._s_tentry};SSOLoginState={cookie.SSOLoginState};SUHB={cookie.SUHB};SUBP={cookie.SUBP};un={cookie.un};ALF={cookie.ALF};");
            request.Headers.Add("Cookie", $"SUB={cookie.SUB};SSOLoginState={cookie.SSOLoginState};SUHB={cookie.SUHB};SUBP={cookie.SUBP};ALF={cookie.ALF};");

            var re = await hc.SendAsync(request);
            var cccc = re.Content.Headers.ContentDisposition;
            var respond = await re.Content.ReadAsByteArrayAsync();
            var file = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("img.jpg", CreationCollisionOption.ReplaceExisting);
            var openstream = await file.OpenStreamForWriteAsync();
            await openstream.WriteAsync(respond, 0, respond.Length);
            openstream.Dispose();
        }
    }
}
