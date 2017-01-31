using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace xiaoiceuwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //cookie
        public static SinaCookie sinacookie;
        //小冰的新浪微博userid，可以换成其他用户的userid，同样可以进行私信
        const long xiaoiceid = 5175429989;
        public MainPage()
        {
            this.InitializeComponent();
            //sinacookie = new SinaCookie();
        }
        //发送私信
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sinacookie == null)
                sinacookie = await GetCookieValue(false);
            if (sinacookie != null)
            {
                await MsgManager.PostMsg(sinacookie, xiaoiceid, inputmsg.Text);
                string respond = await MsgManager.GetMsg(sinacookie, xiaoiceid);
                respondmsg.Text = respond;
                return;
            }
            await new MessageDialog("请手动登录").ShowAsync();
        }
        //获取对方私信回复
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (sinacookie == null)
                sinacookie = await GetCookieValue(false);
            if (sinacookie != null)
            {
                string respond = await MsgManager.GetMsg(sinacookie, xiaoiceid);
                respondmsg.Text = respond;
                return;
            }
            await new MessageDialog("请手动登录").ShowAsync();

        }
        //手动登录
        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            sinacookie = await GetCookieValue(true);
        }
        /// <summary>
        /// 获取登录cookie
        /// </summary>
        /// <param name="forcelogin">是否强制重新登录</param>
        /// <returns></returns>
        private async Task<SinaCookie> GetCookieValue(bool forcelogin)
        {
            string filename = "sinacookie.json";
            Windows.Storage.StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await folder.TryGetItemAsync(filename);
            if (forcelogin)
            {
                file = null;
            }
            //没有cookies文件
            if (file == null)
            {
                //显示登录对话框
                ContentDialogResult result = await logindialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    WeiboLogin wb = new WeiboLogin(account.Text, password.Password);
                    SoftwareBitmapSource pinImage = await wb.Start();
                    string loginresult = "登录失败";
                    //需要验证码
                    if (pinImage != null)
                    {
                        pinimg.Source = pinImage;//显示验证码
                        //输入验证码
                        ContentDialogResult ctresult = await ct.ShowAsync();
                        if (ctresult == ContentDialogResult.Primary)
                        {
                            loginresult = await wb.End(pin.Text);//使用验证码登录
                        }
                        else
                        {
                            loginresult = "用户没有输入验证码，请重新登录";
                        }
                    }
                    //不需要验证码
                    else
                    {
                        loginresult = await wb.End(null);
                    }

                    //登录结果判断
                    if (loginresult == "0")
                    {
                        await new MessageDialog("登录成功！").ShowAsync();
                        sinacookie.un = account.Text;
                        sinacookie.wvr = "6";
                        sinacookie._s_tentry = "login.sina.com.cn";
                        sinacookie.UOR = "login.sina.com.cn";
                        //将获得的cookies持久化到文件
                        try
                        {
                            string serializedcookie = JsonConvert.SerializeObject(sinacookie);
                            Windows.Storage.StorageFile cookiefile = await folder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                            await Windows.Storage.FileIO.WriteTextAsync(cookiefile, serializedcookie);
                            return sinacookie;
                        }
                        catch (Exception ex)
                        {
                            await new MessageDialog(ex.Message).ShowAsync();
                            return null;
                        }
                    }
                    else if (loginresult == "2070")
                    {
                        await new MessageDialog("验证码错误，请重新登录").ShowAsync();
                    }
                    else if (loginresult == "101&")
                    {
                        await new MessageDialog("密码错误，请重新登录").ShowAsync();
                    }
                    else if (loginresult == "4049")
                    {
                        await new MessageDialog("验证码为空，请重新登录").ShowAsync();
                    }
                    else
                    {
                        await new MessageDialog(loginresult).ShowAsync();
                    }
                    return null;
                }
                //没有登录
                else
                {
                    await new MessageDialog("请重新登录").ShowAsync();
                    return null;
                }
            }
            //从文件反序列化cookies
            else
            {
                try
                {
                    Windows.Storage.StorageFile cookie = await folder.GetFileAsync(filename);
                    string text = await Windows.Storage.FileIO.ReadTextAsync(cookie);
                    SinaCookie cookiefile = JsonConvert.DeserializeObject<SinaCookie>(text);
                    return cookiefile;
                }
                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message).ShowAsync();
                    return null;
                }
            }
        }
    }
}
