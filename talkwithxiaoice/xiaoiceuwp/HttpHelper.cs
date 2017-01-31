using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;

namespace xiaoiceuwp
{
    public class HttpHelper
    {
        public static async Task<string> Get(string url, CookieContainer cookieContainer, bool autoRedirect)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.AllowAutoRedirect = autoRedirect;
            HttpClient hc = new HttpClient(handler);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            handler.CookieContainer = cookieContainer;
            HttpResponseMessage respond = null;
            try
            {
                respond = await hc.SendAsync(request);
                return await respond.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public static async Task<string> Get(string url)
        {
            HttpClient hc = new HttpClient();
            try
            {
                HttpResponseMessage respond = await hc.GetAsync(url);
                return await respond.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        public static async Task<SoftwareBitmapSource> GetImage(string url)
        {
            try
            {

            HttpClient hc = new HttpClient();
            HttpResponseMessage respond = await hc.GetAsync(url);
            Stream stream = await respond.Content.ReadAsStreamAsync();
            MemoryStream mstream = new MemoryStream();
            await stream.CopyToAsync(mstream);
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(mstream.AsRandomAccessStream());
            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
            if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || softwareBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
            {
                softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            }
            var source = new SoftwareBitmapSource();
            await source.SetBitmapAsync(softwareBitmap);
            return source;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        public static async Task<string> Post(string url, string data)
        {
            try
            {
                Windows.Web.Http.HttpClient hccp = new Windows.Web.Http.HttpClient();
                Windows.Web.Http.HttpRequestMessage hrm = new Windows.Web.Http.HttpRequestMessage(Windows.Web.Http.HttpMethod.Post, new Uri(url));
                hrm.Content = new Windows.Web.Http.HttpStringContent(data);
                hrm.Content.Headers.ContentType = new Windows.Web.Http.Headers.HttpMediaTypeHeaderValue("application/x-www-form-urlencoded");
                var rsp = await hccp.SendRequestAsync(hrm);
                string[] s = rsp.Headers["set-Cookie"].Split(',');
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i].StartsWith(" SUBP"))
                    {
                        MainPage.sinacookie.SUBP = s[i].Split(';')[0].Substring(6);
                    }
                    else if (s[i].StartsWith(" SUB"))
                    {
                        MainPage.sinacookie.SUB = s[i].Substring(5, 86);
                    }
                    else if (s[i].StartsWith(" ALF"))
                    {
                        MainPage.sinacookie.ALF = s[i].Substring(5, 10);
                    }
                }
                return await rsp.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
