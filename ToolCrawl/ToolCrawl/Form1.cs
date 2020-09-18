using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using xNet;
using DevExpress.XtraSplashScreen;
namespace ToolCrawl
{
    public partial class Form1 : Form
    {
        HttpRequest http = new HttpRequest();
        HttpClient httpClient = new HttpClient();
        WebClient webClient = new WebClient();
        public Form1()
        {
            InitializeComponent();
        }
        void Crawl()
        {
            try
            {
                string ml = textBox1.Text;
                string html = http.Get(@ml).ToString();
                string ChapterPattern = @"(?<=<div class=""col-xs-5 chapter"">).*?(?=</div>)";
                string TenPattern = @"(?<=<div class=""mrt10"" itemscope="""" itemtype=""http://schema.org/Book"">).*?(?=</div>)";
                var list = Regex.Matches(html, ChapterPattern, RegexOptions.Singleline);
                var Ten = Regex.Matches(html, TenPattern, RegexOptions.Singleline);
                var catTen = Regex.Match(Ten[0].ToString(), @"(?<=<span itemprop=""name"">).*?(?=</span>)", RegexOptions.Singleline);
                labelTap.Text = catTen.ToString();
                for (int i = 0; i < list.Count; i++)
                {
                    string pattern = @"(?<=<a href="").*?(?="" data)";
                    var catChuoi = Regex.Match(list[i].ToString(), pattern, RegexOptions.Singleline);
                    string Tappattern = @"(?<="">).*?(?=<)";
                    var catTap = Regex.Match(list[i].ToString(), Tappattern, RegexOptions.Singleline);
                    string chuoi = catChuoi.ToString();
                    string tap = catTap.ToString();
                    ListViewItem item1 = new ListViewItem(tap);
                    item1.SubItems.Add(chuoi);
                    listView1.Items.Add(item1);
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Tải thất bại");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("bạn chưa nhập link vào");
                return;
            }
            listView1.Items.Clear();
            textBox2.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = true;
            Crawl();
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count >= 1)
            {
                label4.Text = listView1.SelectedItems[0].SubItems[0].Text;
                textBox2.Text = listView1.SelectedItems[0].SubItems[1].Text;
                textBox2.Enabled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // MessageBox.Show("Tải thành công");
        }

        private void WebClient_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            Invoke(new MethodInvoker(delegate ()
            {
                progressBar1.Minimum = 0;
                double receive = double.Parse(e.BytesReceived.ToString());
                double total = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = receive / total * 100;
                labelDown.Text = $"Đang tải {string.Format("{0:0.##}", percentage)}%";
                progressBar1.Value = int.Parse(Math.Truncate(percentage).ToString());
            }));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            downAnh(textBox2.Text);
        }
        async void downAnh(string linkChapter)
        {
            try
            {
                string ml = linkChapter;
                string html = http.Get(@ml).ToString();
                var list = Regex.Matches(html, @"(?<=<div class=""reading-detail box_doc"">).*?(?=<div class=""container"">)", RegexOptions.Singleline);
                var list2 = Regex.Matches(list[0].ToString(), @"(?<=<div id='page).*?(?=</div>)", RegexOptions.Singleline);
                string ten = labelTap.Text + "\\";
                string tap = label4.Text + "\\";
                
                var charsToRemove = new string[] { ":", "?" };
                foreach (var c in charsToRemove)
                {
                    tap = tap.Replace(c, string.Empty);
                }
                foreach (var c in charsToRemove)
                {
                    ten = ten.Replace(c, string.Empty);
                }
                String server = Environment.UserName;
                Directory.CreateDirectory(@"C:\Users\" + server + "\\Desktop\\Crawl_Img\\" + ten + tap);
                int chapt = 0;
                int dem = list2.Count;
                for (int i = 0; i < list2.Count; i++)
                {
                    string a = list2[i].ToString();
                    var link = Regex.Matches(a, @"(?<=src=').*?(?=' data)", RegexOptions.Singleline);
                    string generatedFileName = @"C:\Users\" + server + "\\Desktop\\Crawl_Img\\" + ten + tap + chapt + ".jpg";
                    string getlink = link[0].ToString();

                    progressBar1.Value = 0;
                    webClient.Credentials = CredentialCache.DefaultNetworkCredentials;
                    webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                    Uri uri = new Uri(getlink);
                    webClient.Headers.Add("Referer", linkChapter);
                    webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.135 Safari/537.36");
                    await webClient.DownloadFileTaskAsync(uri, generatedFileName);
                    chapt = chapt + 1;
                    label6.Text = "Downloading [ " + chapt.ToString() + " / " + dem.ToString() + " ]";
                }
                MessageBox.Show("Tải thành công");
            }
            catch (Exception)
            {

                MessageBox.Show("Lỗi");
            }
        }
        void downTatCaAnh(string linkChapter,string Chapter)
        {
            try
            {
                string ml = linkChapter;
                string html = http.Get(@ml).ToString();
                var list = Regex.Matches(html, @"(?<=<div class=""reading-detail box_doc"">).*?(?=<div class=""container"">)", RegexOptions.Singleline);
                var list2 = Regex.Matches(list[0].ToString(), @"(?<=<div id='page).*?(?=</div>)", RegexOptions.Singleline);
                string ten = labelTap.Text + "\\";
                string tap = Chapter + "\\";
                var charsToRemove = new string[] { ":", "?","/" };
                foreach (var c in charsToRemove)
                {
                    tap = tap.Replace(c, string.Empty);
                }
                foreach (var c in charsToRemove)
                {
                    ten = ten.Replace(c, string.Empty);
                }
                String server = Environment.UserName;
                Directory.CreateDirectory(@"C:\Users\" + server + "\\Desktop\\Crawl_Img\\" + ten + tap);
                int chapt = 0;
                int dem = list2.Count;
                for (int i = 0; i < list2.Count; i++)
                {
                    string a = list2[i].ToString();
                    var link = Regex.Matches(a, @"(?<=src=').*?(?=' data)", RegexOptions.Singleline);
                    string generatedFileName = @"C:\Users\" + server + "\\Desktop\\Crawl_Img\\" + ten + tap + chapt + ".jpg";
                    string getlink = link[0].ToString();

                    progressBar1.Value = 0;
                    webClient.Credentials = CredentialCache.DefaultNetworkCredentials;
                    webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                    Uri uri = new Uri(getlink);
                    webClient.Headers.Add("Referer", linkChapter);
                    webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.135 Safari/537.36");
                    webClient.DownloadFile(uri, generatedFileName);
                    chapt = chapt + 1;
                    label6.Text = "Downloading [ " + chapt.ToString() + " / " + dem.ToString() + " ]";
                }
                
            }
            catch (Exception)
            {

                MessageBox.Show("Lỗi");
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            SplashScreenManager.ShowDefaultWaitForm();
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                string chapter = listView1.Items[i].SubItems[0].Text;
                string link = listView1.Items[i].SubItems[1].Text;
                chapter = chapter.Replace(":", string.Empty);
                downTatCaAnh(link, chapter);
            }
            SplashScreenManager.CloseDefaultSplashScreen();
            MessageBox.Show("Tải thành công");
        }
    }
}
