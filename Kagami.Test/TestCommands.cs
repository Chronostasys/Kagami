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
using FluentFTP;

namespace Kagami.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }
    [Test]
    public async Task OnPixivCrawl()
    {
        var c = new PixivAppAPI();
        var aure = await c.AuthAsync("1WRRkxi2fNjvrY4ZcMFbyw5sOxnMf2uJojd5UjsCs7w");
        await File.WriteAllTextAsync("refreshtoken",c.RefreshToken);
        // var rec = await c.GetIllustRecommendedAsync();
        // var img = rec.Illusts.First();
        var re = await c.GetIllustDetailAsync("88340806");
        var ug = await c.GetUgoiraMetadataAsync("88340806");
        await c.DownloadAsync(ug.UgoiraMetadataUgoiraMetadata.ZipUrls.Medium.ToString(),"1.zip");
        await c.DownloadAsync(re.Illust.MetaSinglePage.OriginalImageUrl.ToString(),"2.jpg");

        Assert.Pass();
    }
}