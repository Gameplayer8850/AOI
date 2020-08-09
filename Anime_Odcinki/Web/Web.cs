using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Security.Cryptography;

namespace Anime_Odcinki.Web
{
    class Web
    {
        public Boolean AnimeZone_Available()
        {
            return Link_Available("https://www.animezone.pl/");
        }
        public Boolean MAL_Available()
        {
            return Link_Available("https://myanimelist.net");
        }
        public Boolean MAL_Acc_Exist(String nick) {
            return Link_Available("https://myanimelist.net/animelist/" + nick);
        }
        private Boolean Link_Available(String link) {
            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(link);
                HttpWebResponse httpRes = (HttpWebResponse)httpReq.GetResponse();
                httpRes.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }


        public String ListHtmlCode() {
            try
            {
                using (WebClient client = new WebClient())
                {
                    String htmlCode = client.DownloadString("https://myanimelist.net/animelist/"+Data.Data.nick_MAL+"/load.json?status=1");
                    return htmlCode;
                }
            }
            catch
            {
                return "";
            }
        }
        public String HtmlCode(String link)
        {
            try
            {
                HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(link);
                request1.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                HttpWebResponse getResponse1 = (HttpWebResponse)request1.GetResponse();
                using (StreamReader sr = new StreamReader(getResponse1.GetResponseStream()))
                {
                    string sourceCode = sr.ReadToEnd();
                    return sourceCode;
                }
            }
            catch
            {
                return "";
            }
        }
        public byte[] Get_Image_in_Byte(String link)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    byte[] imageData = client.DownloadData(link);
                    return imageData;
                }
            }
            catch {
                return null;
            }
        }
    }
}
