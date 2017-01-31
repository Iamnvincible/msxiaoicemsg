using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xiaoiceuwp
{
    public class SinaCookie
    {
        //并无大碍
        //public string SINAGLOBAL;
        //固定值“6”
        public string wvr;
        //用户名
        public string un;
        ////十年不变,非必要
        //public string SCF;
        //重要，在POST的返回头set-cookies中获取
        public string SUB;
        //在POST的返回头set-cookies中获取
        public string SUBP;
        //在get的cookies中获取
        public string SUHB;
        //在get的cookies中获取
        public string ALF;
        //在post的返回头set-cookies中获取
        public string SSOLoginState;
        //固定值login.sina.com.cn
        public string _s_tentry;
        //固定值login.sina.com.cn
        public string UOR;
        ////并无大碍
        //public string Apache;
        ////并无大碍
        //public string ULV;
    }
}
