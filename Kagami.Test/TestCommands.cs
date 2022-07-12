using Kagami.Function;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using NUnit.Framework;
using PixivCS;
using System;
using System.IO;
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
        var c = new Crawler();

        Assert.Pass();
    }
    [Test]
    public async Task OnPixivCrawl()
    {
        var c = new PixivAppAPI();
        await c.AuthAsync("1WRRkxi2fNjvrY4ZcMFbyw5sOxnMf2uJojd5UjsCs7w");
        await File.WriteAllTextAsync("refreshtoken",c.RefreshToken);
        var rec = await c.GetIllustRecommendedAsync();

        await c.DownloadAsync(rec.Illusts[0].ImageUrls.Large.ToString(),"1.jpg");

        Assert.Pass();
    }
}