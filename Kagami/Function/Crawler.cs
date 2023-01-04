using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abot2.Core;
using Abot2.Crawler;
using Abot2.Poco;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using Serilog;

namespace Kagami.Function
{
    public class Crawler
    {
        static Regex regex = new(@"^magnet:\?xt=urn:btih:[0-9a-fA-F]{40,}.*$");
        static HttpClient client = new HttpClient();

        public async Task DemoSimpleCrawler()
        {
            var config = new CrawlConfiguration
            {
                MaxPagesToCrawl = 10, //Only crawl 10 pages
                MinCrawlDelayPerDomainMilliSeconds = 3000, //Wait this many millisecs between requests
            };
            var crawler = new PoliteWebCrawler(config);

            crawler.PageCrawlCompleted += PageCrawlCompleted;//Several events available...

            var crawlResult = await crawler.CrawlAsync(new Uri("https://thepiratebay.org/search.php?q=windows&cat=0"));
            
        }

        public static async Task<MessageBuilder> DemoSinglePageRequest(TextChain chain)
        {
            var text = chain.Content[7..].Trim();
            var cat = "0";
            if (text.StartsWith("-c"))
            {
                cat = $"{text[3]}00";
                text = text[4..].Trim();
            }
            var re = await client.GetFromJsonAsync<IEnumerable<JSONInfo>>($"https://apibay.org/q.php?q={text}&cat={cat}");
            var result = new MessageBuilder();
            if (re==null || !re.Any())
            {
                result.Text("无对应资源。提示：只支持搜索纯英文");
                return result;
            }
            re = re.Take(10);
            // Build message
            var i = 0;
            foreach (var item in re)
            {
                Console.WriteLine(item.Name);
                i++;
                // result.Text("名："+item.Name+"\n");
                var link = $"magnet:?xt=urn:btih:{item.Info_Hash}";
                result.Text($"BASE64: {Convert.ToBase64String(Encoding.UTF8.GetBytes(link))}\n");
                // result.Text($"类：{Cat[item.Category[0]]}\n");
                // result.Text($"大小：{SizeSuffix(item.Size)}\n\n");
            }
            Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            Console.WriteLine(i);
            return result;

        }

        private void PageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            if (crawledPage.HttpRequestException != null || crawledPage.HttpResponseMessage.StatusCode != HttpStatusCode.OK)
                return;
            else
                Console.WriteLine($"Crawl of page succeeded {crawledPage.Uri.AbsoluteUri}");

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
                return;

            var matches = regex.Matches(crawledPage.Content.Text);
            foreach (var item in matches)
            {
                var s = item.ToString();
                Console.WriteLine(s);
            }
        }
        static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
        public static Dictionary<char, string> Cat = new Dictionary<char, string>()
        {
            { '0',"未知" },
            { '1',"Audio" },
            { '2',"Video" },
            { '3',"Applications" },
            { '4',"Games" },
            { '5',"Porn" },
            { '6',"Other" },
        };
    }
    class JSONInfo
    {
        public string Info_Hash { get; set; }
        public string Name { get; set; }
        public string Category { get;set; }
        public Int64 Size { get; set; }
    }
}
