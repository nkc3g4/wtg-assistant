using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace wintogo
{

    public class SWOnline
    {

        public SWOnline()
        {


        }
        public SWOnline(string releaseUrl)
        {
            this.ReleaseUrl = releaseUrl;
        }
        private string releaseUrl;

        public string ReleaseUrl
        {
            get { return releaseUrl; }
            set { releaseUrl = value; }
        }

        private string[] topicLink;

        public string[] TopicLink
        {
            get { return topicLink; }
            set { topicLink = value; }
        }
        private string[] topicName;

        public string[] TopicName
        {
            get { return topicName; }
            set { topicName = value; }
        }

        private LinkLabel linkLabel;

        public LinkLabel Linklabel
        {
            get { return linkLabel; }
            set { linkLabel = value; }
        }

        //public void Report()
        //{
        //    string pageHtml;
        //    try
        //    {

        //        WebClient MyWebClient = new WebClient();

        //        MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于对向Internet资源的请求进行身份验证的网络凭据。

        //        Byte[] pageData = MyWebClient.DownloadData(releaseUrl); //从指定网站下载数据
        //        pageHtml = Encoding.Default.GetString(pageData);
        //        //MessageBox.Show(pageHtml);
        //        int index = pageHtml.IndexOf("webreport=");
        //        if (pageHtml.Substring(index + 10, 1) == "1")
        //        {
        //            //string strURL = "http://myapp.luobotou.org/statistics.aspx?name=wtg&ver=" + Application.ProductVersion;

        //            string strURL = reportUrl + Application.ProductVersion;
        //            HttpWebRequest request;
        //            request = (HttpWebRequest)WebRequest.Create(strURL);
        //            HttpWebResponse response;
        //            response = (HttpWebResponse)request.GetResponse();
        //            using (StreamReader myreader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
        //            {
        //                string responseText = myreader.ReadToEnd();
        //            }
        //        }


        //    }
        //    catch (WebException webEx)
        //    {

        //        Log.WriteLog("Err_ReportErr", webEx.ToString());

        //    }
        //}
        public void Update()
        {
            //string autoup = IniFile.ReadVal("Main", "AutoUpdate", Application.StartupPath + "\\files\\settings.ini");
            //if (autoup == "0") { return; }
            string pageHtml;
            try
            {
                WebClient MyWebClient = new WebClient();
                //MyWebClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                MyWebClient.Credentials = CredentialCache.DefaultCredentials;
                byte[] pageData = MyWebClient.DownloadData(releaseUrl); //从指定网站下载数据"https://bbs.luobotou.org/app/wintogo.txt"

                pageHtml = Encoding.UTF8.GetString(pageData);
                int index = pageHtml.IndexOf("~");
                Version newVer = new Version(pageHtml.Substring(index + 1, 7));
                Version currentVer = new Version(Application.ProductVersion);
                
                if (newVer > currentVer)
                {
                    Update frmf = new Update(newVer.ToString());
                    frmf.ShowDialog();
                }

            }
            catch (WebException webEx)
            {
                Log.WriteLog("Err_UpdateErr", webEx.ToString());
            }
        }



        public void Showad()
        {
            string pageHtml;
            try
            {
                WebClient MyWebClient = new WebClient();

                MyWebClient.Credentials = CredentialCache.DefaultCredentials;
                
                byte[] pageData = MyWebClient.DownloadData("https://bbs.luobotou.org/app/wintogo.txt"); //从指定网站下载数据
                //MyWebClient.DownloadString()
                pageHtml = Encoding.UTF8.GetString(pageData);
                int index = pageHtml.IndexOf("announcement=");
                int indexbbs = pageHtml.IndexOf("bbs=");
                if (pageHtml.Substring(index + 13, 1) != "0" && MsgManager.ci.EnglishName != "English")
                {
                    if (pageHtml.Substring(indexbbs + 4, 1) == "1")
                    {
                        //string pageHtml;
                        //WebClient MyWebClient2 = new WebClient();
                        MyWebClient.Credentials = CredentialCache.DefaultCredentials;
                        byte[] pageData1 = MyWebClient.DownloadData("https://bbs.luobotou.org/portal.php");
                        pageHtml = Encoding.UTF8.GetString(pageData1);

                        #region 正则表达式实现
                     
                        Match matchArticles = Regex.Match(pageHtml, @"<ul><li><a href=[\W\w]+?</li></ul>");
                        MatchCollection matches = Regex.Matches(matchArticles.Groups[0].Value, @"<li><a href=""(.+?)"".+?>(.+?)</a></li>");
               
                        for (int i = 0; i < matches.Count; i++)
                        {
                            TopicLink[i] = matches[i].Groups[1].Value;
                            TopicName[i] = matches[i].Groups[2].Value;
                            //Console.WriteLine(TopicName[i]);

                        }
                        #endregion



                        //int index1 = pageHtml.IndexOf("<ul><li><a href=");
                        //for (int i = 0; i < 10; i++)
                        //{

                        //    int LinkStartIndex = pageHtml.IndexOf("<li><a href=", index1) + 13;
                        //    int LinkEndIndex = pageHtml.IndexOf("\"", LinkStartIndex);
                        //    int TitleStartIndex = pageHtml.IndexOf("title=", LinkEndIndex) + 7;
                        //    int TitleEndIndex = pageHtml.IndexOf("\"", TitleStartIndex);
                        //    topicLink[i] = pageHtml.Substring(LinkStartIndex, LinkEndIndex - LinkStartIndex);
                        //    topicName[i] = pageHtml.Substring(TitleStartIndex, TitleEndIndex - TitleStartIndex);
                        //    index1 = LinkEndIndex;
                        //    //topicstring 
                        //    //int adprogram = index1 + Application.ProductName.Length + 1;
                        //}
                    }
                    pageHtml = MyWebClient.DownloadString("https://bbs.luobotou.org/app/announcement.txt");
                    Match match = Regex.Match(pageHtml, Application.ProductName + "=(.+)~(.+)结束");
                    string adlink = match.Groups[2].Value;
                    string adtitle = match.Groups[1].Value;
                    linkLabel.Invoke(new Action(() => 
                    {
                        linkLabel.Text = adtitle;
                        linkLabel.Visible = true;
                    }));
                    linkLabel.Tag = adlink;
                }
            }
            catch (Exception ex)
            {

                Log.WriteLog("Err_ShowAdError", ex.ToString());
            }
        }
    }
}
