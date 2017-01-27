using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace talkwithxiaoice
{
    public class Respond
    {
        public int total { get; set; }
        public int ok { get; set; }
        public string msg { get; set; }
        public int maxPage { get; set; }
        public Datum[] data { get; set; }
        public int blocked { get; set; }
        public User user { get; set; }
    }

    public class User
    {
        public string id { get; set; }
        public string description { get; set; }
        public string profile_url { get; set; }
        public bool verified { get; set; }
        public int verified_type { get; set; }
        public int attNum { get; set; }
        public string nativePlace { get; set; }
        public int mblogNum { get; set; }
        public int fansNum { get; set; }
        public bool following { get; set; }
        public bool follow_me { get; set; }
        public int favourites_count { get; set; }
        public string verified_reason { get; set; }
        public string name { get; set; }
        public string profile_image_url { get; set; }
        public string avatar_hd { get; set; }
        public int ptype { get; set; }
        public string created_at { get; set; }
        public H5icon h5icon { get; set; }
        public int mbtype { get; set; }
        public string genderIcon { get; set; }
        public string ta { get; set; }
        public int mbrank { get; set; }
        public int urank { get; set; }
        public Badge badge { get; set; }
    }

    public class H5icon
    {
        public string main { get; set; }
        public object[] other { get; set; }
    }

    public class Badge
    {
        public int uc_domain { get; set; }
        public int enterprise { get; set; }
        public int anniversary { get; set; }
        public int taobao { get; set; }
        public int travel2013 { get; set; }
        public int gongyi { get; set; }
        public int gongyi_level { get; set; }
        public int bind_taobao { get; set; }
        public int hongbao_2014 { get; set; }
        public int suishoupai_2014 { get; set; }
        public int dailv { get; set; }
        public int zongyiji { get; set; }
        public int vip_activity1 { get; set; }
        public int unread_pool { get; set; }
        public int daiyan { get; set; }
        public int ali_1688 { get; set; }
        public int vip_activity2 { get; set; }
        public int suishoupai_2016 { get; set; }
        public int fools_day_2016 { get; set; }
        public int uefa_euro_2016 { get; set; }
        public int super_star_2016 { get; set; }
        public int unread_pool_ext { get; set; }
        public int self_media { get; set; }
        public int olympic_2016 { get; set; }
        public int dzwbqlx_2016 { get; set; }
        public int discount_2016 { get; set; }
        public int wedding_2016 { get; set; }
        public int shuang11_2016 { get; set; }
        public int follow_whitelist_video { get; set; }
        public int wbzy_2016 { get; set; }
        public int hongbao_2017 { get; set; }
        public int hongbao_2017_2 { get; set; }
        public int caishen_2017 { get; set; }
    }

    public class Datum
    {
        public long id { get; set; }
        public string idstr { get; set; }
        public string created_at { get; set; }
        public string text { get; set; }
        public int sys_type { get; set; }
        public int msg_status { get; set; }
        public long sender_id { get; set; }
        public long recipient_id { get; set; }
        public string sender_screen_name { get; set; }
        public string recipient_screen_name { get; set; }
        public Sender sender { get; set; }
        public Recipient recipient { get; set; }
        public string mid { get; set; }
        public bool isLargeDm { get; set; }
        public string source { get; set; }
        public int status_id { get; set; }
        public object geo { get; set; }
        public int dm_type { get; set; }
        public int media_type { get; set; }
        public int ip { get; set; }
        public int burn_time { get; set; }
        public Ext_Text ext_text { get; set; }
        public bool matchKeyword { get; set; }
        public bool topublic { get; set; }
        public bool pushToMPS { get; set; }
        public int oriImageId { get; set; }
        public int recall_status { get; set; }
        public int self { get; set; }
        public long[] att_ids { get; set; }
        public Attachment[] attachment { get; set; }
    }

    public class Sender
    {
        public long id { get; set; }
        public string screen_name { get; set; }
        public string profile_image_url { get; set; }
        public bool verified { get; set; }
        public int verified_type { get; set; }
        public int mbtype { get; set; }
        public string profile_url { get; set; }
        public H5icon1 h5icon { get; set; }
    }

    public class H5icon1
    {
        public object main { get; set; }
        public string[] other { get; set; }
    }

    public class Recipient
    {
        public long id { get; set; }
        public string screen_name { get; set; }
        public string profile_image_url { get; set; }
        public bool verified { get; set; }
        public int verified_type { get; set; }
        public int mbtype { get; set; }
        public string profile_url { get; set; }
        public H5icon2 h5icon { get; set; }
    }

    public class H5icon2
    {
        public object main { get; set; }
        public string[] other { get; set; }
    }

    public class Ext_Text
    {
        public bool autoReply { get; set; }
        public string sender_box_type { get; set; }
    }

    public class Attachment
    {
        public long fid { get; set; }
        public long vfid { get; set; }
        public string filename { get; set; }
        public int filesize { get; set; }
        public string extension { get; set; }
        public string soundtime { get; set; }
        public string size { get; set; }
    }
}
