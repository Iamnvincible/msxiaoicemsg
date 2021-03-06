﻿using System;
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
using Windows.Media.Playback;
using Windows.Media.Core;
using Windows.Graphics.Imaging;

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
        public static long xiaoiceid = 5175429989;
        public MainPage()
        {
            this.InitializeComponent();
        }
        //发送私信
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sinacookie == null)
                sinacookie = await GetCookieValue(false);
            if (sinacookie != null)
            {
                bool issendsuccess = await MsgManager.PostMsg(sinacookie, xiaoiceid, inputmsg.Text);
                if (issendsuccess)
                {
                    SimpleRespond respond = await MsgManager.GetMsg(sinacookie, xiaoiceid);
                    if (respond == null)
                        respond = new SimpleRespond();
                    respondmsg.Text = respond.Message;
                    await PlayVoiceorSetImage(respond.Message);
                }
                else
                {
                    await new MessageDialog("cookie 过期或网络异常，请重新登录").ShowAsync();
                }
            }
        }
        //获取对方私信回复
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (sinacookie == null)
                sinacookie = await GetCookieValue(false);
            if (sinacookie != null)
            {
                SimpleRespond respond = await MsgManager.GetMsg(sinacookie, xiaoiceid);
                if (respond == null)
                    respond = new SimpleRespond();
                respondmsg.Text = respond.Message;
                await PlayVoiceorSetImage(respond.Message);
            }
        }

        private void Player_MediaEnded(MediaPlayer sender, object args)
        {
            sender.Dispose();
        }
        private async Task PlayVoiceorSetImage(string msg)
        {
            if (msg == "分享语音")
            {
                try
                {

                    MediaPlayer player = new MediaPlayer();
                    var file = await Windows.Storage.ApplicationData.Current.LocalCacheFolder.GetFileAsync("ttsvoice.mp3");

                    player.Source = MediaSource.CreateFromStorageFile(file);
                    player.MediaEnded += Player_MediaEnded;
                    player.Play();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    respondmsg.Text = "语音播放失败";
                }
            }
            if (msg == "分享图片")
            {
                try
                {
                    var file = await Windows.Storage.ApplicationData.Current.LocalCacheFolder.GetFileAsync("img.jpg");
                    var istream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(istream);
                    SoftwareBitmap softwarebitmap = await decoder.GetSoftwareBitmapAsync();
                    if (softwarebitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || softwarebitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
                    {
                        softwarebitmap = SoftwareBitmap.Convert(softwarebitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    }
                    var source = new SoftwareBitmapSource();
                    await source.SetBitmapAsync(softwarebitmap);
                    shareimg.Source = source;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    respondmsg.Text = "图片无法显示";
                }
            }
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
                if (sinacookie == null)
                {
                    sinacookie = new SinaCookie();
                }
                //显示登录对话框
                ContentDialogResult result = await logindialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    //WeiboLogin wb = new WeiboLogin(account.Text, password.Password);
                    Mweibologin wb = new Mweibologin(account.Text, password.Password);
                    string loginresult = await wb.LoginAsync();
                    //SoftwareBitmapSource pinImage = await wb.Start();
                    //string loginresult = "登录失败";
                    ////需要验证码
                    //if (pinImage != null)
                    //{
                    //    pinimg.Source = pinImage;//显示验证码
                    //    //输入验证码
                    //    ContentDialogResult ctresult = await ct.ShowAsync();
                    //    if (ctresult == ContentDialogResult.Primary)
                    //    {
                    //        loginresult = await wb.End(pin.Text);//使用验证码登录
                    //    }
                    //    else
                    //    {
                    //        loginresult = "用户没有输入验证码，请重新登录";
                    //    }
                    //}
                    ////不需要验证码
                    //else
                    //{
                    //    loginresult = await wb.End(null);
                    //}

                    //登录结果判断
                    if (loginresult == "200")
                    {
                        await new MessageDialog("登录成功！").ShowAsync();
                        //sinacookie.un = account.Text;
                        //sinacookie.wvr = "6";
                        //sinacookie._s_tentry = "login.sina.com.cn";
                        //sinacookie.UOR = "login.sina.com.cn";
                        sinacookie._T_WM = "37bd0cf59fe89681339839225c2bb4d7";
                        sinacookie.SCF = "AgNWq-vwodzezz_wenjZ2Y_ohsIl-Ri8pDBNo4nlBli4VdB9htXwYYQmOap96JTsAG8Ie3iRNeLLy82Nf1H6R5U.";
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

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                MediaPlayer player = new MediaPlayer();
                var file = await Windows.Storage.ApplicationData.Current.LocalCacheFolder.GetFileAsync("ttsvoice.mp3");
                player.Source = MediaSource.CreateFromStorageFile(file);
                player.Play();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
