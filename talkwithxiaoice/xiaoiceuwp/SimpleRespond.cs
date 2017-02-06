using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xiaoiceuwp
{
    public class SimpleRespond
    {
        private string _voice;

        public string Voice
        {
            get { return _voice; }
            set { _voice = " http://upload.api.weibo.com/2/mss/msget?source=351354573&fid="+value; }
        }

        public string Message { get; set; }
        public string Image { get; set; }
        public SimpleRespond()
        {
            this.Message = "can not retrieve message";
        }
    }
}
