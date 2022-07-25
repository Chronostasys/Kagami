using Kagami.Function;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using NUnit.Framework;
using PixivCS;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kagami.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestPing()
    {
        Console.WriteLine(Command.OnCommandPing
            (TextChain.Create("/ping")).Build().ToString());
        Assert.Pass();
    }

    [Test]
    public async Task OnCommandBvParser()
    {
        var textChain = TextChain
            .Create("BV1Qh411i7ic");
        {
            // Get result
            var result = await Command
                .OnCommandBvParser(textChain);

            Console.WriteLine(result.Build());
        }
        Assert.Pass();
    }

    [Test]
    public async Task OnCommandGithubParser()
    {
        var textChain = TextChain.Create
            ("https://github.com/KonataDev/Kagami");
        {
            // Get result
            var result = await Command
                .OnCommandGithubParser(textChain);

            Console.WriteLine(result.Build());
        }
        Assert.Pass();
    }

    [Test]
    public void OnCommandEcho()
    {
        var textChain = TextChain.Create("/echo =w=");
        var messageChain = new MessageBuilder(textChain);
        {
            // Get result
            var result = Command.OnCommandEcho
                (textChain, messageChain.Build());

            Console.WriteLine(result.Build().ToString());
        }
        Assert.Pass();
    }

    [Test]
    public void OnCommandEval()
    {
        var messageChain = new MessageBuilder
            ("/eval =w=");
        {
            Console.WriteLine(Command.OnCommandEval
                (messageChain.Build()).Build());
        }
        Assert.Pass();
    }
    [Test]
    public async Task OnCrawl()
    {
        var restr = await new HttpClient().GetStringAsync($"https://search.kuwo.cn/r.s?all=zood&ft=music&%20itemset=web_2013&client=kt&pn=0&rn=1&rformat=json&encoding=utf8");
        var nstr = restr.Replace('\'', '"').Trim();
        Console.WriteLine(nstr);
        var re = JsonSerializer.Deserialize<KuwoSearchDto>(nstr);

        Assert.Pass();
    }
    [Test]
    public async Task OnPixivCrawl()
    {
        var c = new PixivAppAPI();
        await c.AuthAsync("1WRRkxi2fNjvrY4ZcMFbyw5sOxnMf2uJojd5UjsCs7w");
        await File.WriteAllTextAsync("refreshtoken",c.RefreshToken);
        var rec = await c.GetIllustRecommendedAsync();
        //var rec = await c.GetIllustRankingAsync();
        var img = rec.Illusts.First();
        var re = await c.GetIllustDetailAsync("99358839");
        
        await c.DownloadAsync(re.Illust.ImageUrls.Large.ToString(),"1.jpg");
        await c.DownloadAsync(re.Illust.MetaSinglePage.OriginalImageUrl.ToString(),"2.jpg");

        Assert.Pass();
    }
}