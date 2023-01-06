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
using NeteaseCloudMusicApi;
using System.Collections.Generic;

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
        var api = new CloudMusicApi();
        string account = "";
        var queries = new Dictionary<string, object>();
        bool isPhone = true;
        queries[isPhone ? "phone" : "email"] = account;
        queries["password"] = "";
        var re1 = await api.RequestAsync(isPhone ? CloudMusicApiProviders.LoginCellphone : CloudMusicApiProviders.Login, queries, false);
        if (!CloudMusicApi.IsSuccess(re1))
            Console.WriteLine("登录失败，账号或密码错误");
        var sre = await api.RequestAsync(CloudMusicApiProviders.Cloudsearch, new Dictionary<string, object>{["keywords"] = "one last kiss"}, false);
        var first = sre["result"]["songs"][0]["id"].ToString();
        var ure = await api.RequestAsync(CloudMusicApiProviders.SongUrlV1, new Dictionary<string, object>{["id"] = first}, false);
        var url = ure["data"][0]["url"].ToString();
        var c = new PixivAppAPI();
        var aure = await c.AuthAsync("1WRRkxi2fNjvrY4ZcMFbyw5sOxnMf2uJojd5UjsCs7w");
        await File.WriteAllTextAsync("refreshtoken", c.RefreshToken);
        // var rec = await c.GetIllustRecommendedAsync();
        // var img = rec.Illusts.First();
        var re = await c.GetIllustDetailAsync("88340806");
        var ug = await c.GetUgoiraMetadataAsync("88340806");
        await c.DownloadAsync(ug.UgoiraMetadataUgoiraMetadata.ZipUrls.Medium.ToString(), "1.zip");
        await c.DownloadAsync(re.Illust.MetaSinglePage.OriginalImageUrl.ToString(), "2.jpg");

        Assert.Pass();
    }
}