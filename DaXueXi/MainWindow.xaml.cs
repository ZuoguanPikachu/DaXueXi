using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace DaXueXi
{
    public partial class MainWindow : Window
    {
        Requests requests = new();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                JToken list = GetList();
                string url = GetLatest(list);
                GetAns(url);
            });
        }

        private JToken GetList()
        {
            string url = "https://api.bilibili.com/x/space/dynamic/search?keyword=青年大学习&mid=597126853&pn=1&ps=30&platform=web";
            Dictionary<string, string> headers = new Dictionary<string, string>() 
            {
                {"referer",  $"https://space.bilibili.com/597126853/search/dynamic?keyword={HttpUtility.UrlEncode("青年大学习")}" },
                {"origin", "https://space.bilibili.com" },
                {"user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.0.0 Safari/537.36" }
            };
            HttpContent response = requests.get(url, headers);
            string content = response.ReadAsStringAsync().Result;

            JObject result = JObject.Parse(content);
            JToken list = result["data"]["cards"];

            return list;
        }

        private string GetLatest(JToken list)
        {
            foreach (JToken item in list)
            {
                if (item["desc"]["type"].Value<int>() == 64)
                {
                    string rid = item["desc"]["rid"].Value<string>();
                    string url = $"https://www.bilibili.com/read/cv{rid}/";
                    
                    return url;
                }
            }
            return string.Empty;
        }

        private void GetAns(string url)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                {"user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.0.0 Safari/537.36" }
            };
            HttpContent response = requests.get(url, headers);
            string content = response.ReadAsStringAsync().Result;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            HtmlNode article = doc.DocumentNode.SelectSingleNode("//div[@id='read-article-holder']");

            bool skip = true;
            Dispatcher.BeginInvoke(new Action(() => 
            {
                foreach (HtmlNode p in article.SelectNodes("p"))
                {
                    string text = p.InnerText;
                    if (skip)
                    {
                        if (text.Contains("青年大学习也已经更新到了"))
                        {
                            skip = false;
                            text = Regex.Match(text, "青年大学习也已经更新到了.*?大家快去看看吧").Groups[0].Value;
                            textBox.Text += text + "\n";
                        }
                    }
                    else
                    {
                        textBox.Text += text + "\n";
                    }
                }
            }));
        }
    }
}
