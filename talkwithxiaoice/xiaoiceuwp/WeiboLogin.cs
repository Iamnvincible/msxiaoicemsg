using ChakraBridge;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Graphics.Canvas;
namespace xiaoiceuwp
{
    public class WeiboLogin
    {
        #region properties
        /// <summary>
        /// 用户名
        /// </summary>
        /// c
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        //存放登陆后的cookie
        private CookieContainer myCookies = new CookieContainer();

        public CookieContainer MyCookies
        {
            get { return myCookies; }
            set { myCookies = value; }
        }

        private const string PUBKEY = "EB2A38568661887FA180BDDB5CABD5F21C7BFD59C090CB2D245A87AC253062882729293E5506350508E7F9AA3BB77F4333231490F915F6D63C55FE2F08A49B353F444AD3993CACC02DB784ABBB8E42A9B1BBFFFB38BE18D78E87A0E41B9B8F73A928EE0CCEE1F6739884B9777E4FE9E88A1BBE495927AC4A799B3181D6442443";
        private const string RSAKV = "1330428213";

        private string su; //加密后的用户名
        private string servertime; //预登录参数1（时间戳）
        private string pcid; //预登录参数2
        private string nonce; //预登录参数3（随机数）
        private string showpin; //预登录参数4（是否需要验证码）

        private bool forcedpin = false;



        private string retcode;
        #endregion

        public WeiboLogin(string username, string password, bool forcedpin = false)
        {
            this.Username = username;
            this.Password = password;
            this.forcedpin = forcedpin;

            //Base64加密用户名
            Encoding myEncoding = Encoding.GetEncoding("utf-8");
            byte[] suByte = myEncoding.GetBytes(Uri.EscapeDataString(username));
            su = Convert.ToBase64String(suByte);
        }
        /// <summary>
        ///  预登录得到所需的参数
        /// </summary>
        private async Task GetParameter()
        {
            string url = "http://login.sina.com.cn/sso/prelogin.php?entry=weibo&callback=sinaSSOController.preloginCallBack&su="
                + su + "&rsakt=mod&checkpin=1&client=ssologin.js(v1.4.18)";
            string content = await HttpHelper.Get(url);
            int pos;
            pos = content.IndexOf("servertime");
            servertime = content.Substring(pos + 12, 10);
            pos = content.IndexOf("pcid");
            pcid = content.Substring(pos + 7, 39);
            pos = content.IndexOf("nonce");
            nonce = content.Substring(pos + 8, 6);
            pos = content.IndexOf("showpin");
            showpin = content.Substring(pos + 9, 1);
            //showpin = "1"; //验证码测试
        }
        public async Task<SoftwareBitmapSource> GetPIN()
        {
            string url = "http://login.sina.com.cn/cgi/pin.php?p=" + pcid;
            return await HttpHelper.GetImage(url);
        }
        public async Task<SoftwareBitmapSource> Start()
        {
            await GetParameter();
            if (showpin == "1" || forcedpin)
            {
                return await GetPIN();
            }
            else
            {
                return null;
            }
        }

        private async Task<string> GetSP(string pwd, string servertime, string nonce, string pubkey)
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///sinaSSOEncoder"));
                string js = await FileIO.ReadTextAsync(file);
                //自定义function进行加密
                js += "function getpass(pwd,servicetime,nonce,rsaPubkey){var RSAKey=new sinaSSOEncoder.RSAKey();RSAKey.setPublic(rsaPubkey,'10001');var password=RSAKey.encrypt([servicetime,nonce].join('\\t')+'\\n'+pwd);return password;}";
                ChakraHost host = new ChakraHost();
                host.RunScript(js);
                // CommunicationManager.
                string sp = host.CallFunctionOverride("getpass", new object[] { pwd, servertime, nonce, pubkey });
                return sp;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        private async Task<CookieContainer> GetCookie(string door)
        {
            CookieContainer myCookieContainer = new CookieContainer();
            string sp = await GetSP(Password, servertime, nonce, PUBKEY);//得到加密后的密码
            string postData = "entry=weibo&gateway=1&from=&savestate=7&useticket=1&pagerefer=&vsnf=1&su=" + su
                            + "&service=miniblog&servertime=" + servertime
                            + "&nonce=" + nonce
                            + "&pwencode=rsa2&rsakv=" + RSAKV + "&sp=" + sp
                            + "&sr=1366*768&encoding=UTF-8&prelt=104&url=http%3A%2F%2Fweibo.com%2Fajaxlogin.php%3Fframelogin%3D1%26callback%3Dparent.sinaSSOController.feedBackUrlCallBack&returntype=META";
            if ((showpin == "1" || forcedpin) && door != null)
            {
                postData += "&pcid=" + pcid + "&door=" + door;
            }
            string content = await HttpHelper.Post("http://login.sina.com.cn/sso/login.php?client=ssologin.js(v1.4.18)", postData);
            int pos = content.IndexOf("retcode=");
            retcode = content.Substring(pos + 8, 1);
            if (retcode == "0")
            {
                pos = content.IndexOf("location.replace");
                string url = content.Substring(pos + 18, 285);
                content = await HttpHelper.Get(url, myCookieContainer, false);
                //content = await HttpHelper.Getweb(url, myCookieContainer, false);
                var cookiecollection = myCookieContainer.GetCookies(new Uri("http://weibo.com"));
                foreach (var item in cookiecollection.Cast<Cookie>())
                {
                    Debug.WriteLine(item.Name + ":" + item.Value);
                    if (item.Name.Equals("SUHB"))
                    {
                        MainPage.sinacookie.SUHB = item.Value;
                    }
                    else if (item.Name.Equals("SSOLoginState"))
                    {
                        MainPage.sinacookie.SSOLoginState = item.Value;
                    }
                }
                return myCookieContainer;
            }
            else
            {
                retcode = content.Substring(pos + 8, 4);
                return null;
            }
        }
        public async Task<string> End(string door)
        {
            myCookies = await GetCookie(door);
            return retcode;

        }
        public async Task<string> Get(string url)
        {
            return await HttpHelper.Get(url, new CookieContainer(), true);
        }
    }
}
