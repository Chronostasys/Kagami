using Kagami.Utils;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Exceptions.Model;
using Konata.Core.Interfaces.Api;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using PixivCS.Objects;
using Konata.Codec;
using Konata.Codec.Audio;
using Konata.Codec.Audio.Codecs;
using NeteaseCloudMusicApi;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedParameter.Local

namespace Kagami.Function;

public static class Command
{
    private static uint _messageCounter;
    private static HttpClient _client = new();
    private static readonly uint spuin = 435230136;

    private static Dictionary<uint, bool> whiteList = new()
    {
        {151887325,true},
        {942065241,true},
        {158441764,true},
        {1054725794,true},
        { 658322302, true },
        { 688301255, true }
    };

    private static Dictionary<uint, bool> blackList = new()
    {
        {1761373255,true},
    };

    private static ConcurrentDictionary<string, bool> img_blackList = new();

    internal static async void OnGroupPromoteAdmin(Bot bot, GroupPromoteAdminEvent pe)
    {
        await bot.GetGroupMemberList(pe.GroupUin, true);
        return;
    }
    /// <summary>
    /// On group message
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="group"></param>
    internal static async void OnGroupMessage(Bot bot, GroupMessageEvent group)
    {
        // Increase
        ++_messageCounter;

        if (group.MemberUin == bot.Uin) return;


        var textChain = group.Chain.GetChain<TextChain>();
        var imgchain = group.Chain.GetChain<ImageChain>();
        var mul = group.Chain.GetChain<MultiMsgChain>();


        if (textChain is null)
        {
            if ((imgchain is not null || mul is not null) && Util.CanIDo(0.005))
            {
                var reply = new MessageBuilder();
                reply.Text("我回民看不得这个");
                await bot.SendGroupMessage(group.GroupUin, reply);
                return;
            }
        }
        try
        {
            MessageBuilder? reply = null;
            if (imgchain is not null && textChain is not null)
            {
                if (textChain.Content.Contains("禁止"))
                {
                    var hash = imgchain.FileHash.ToLower();
                    img_blackList[hash] = true;
                    reply = new MessageBuilder();
                    reply.Text($"lsp准则更新，加入0x{hash}");
                    await bot.SendGroupMessage(group.GroupUin, reply);
                    return;
                }
            }
            if (imgchain is not null)
            {
                var hash = imgchain.FileHash.ToLower();
                if (img_blackList.ContainsKey(hash))
                {
                    await bot.RecallMessage(group.Message);
                    await bot.GroupMuteMember(group.GroupUin, group.MemberUin, 360);
                    reply = new MessageBuilder();
                    reply.Text($"你违反了lsp准则0x{hash}，已被禁言6分钟");
                    reply.At(group.MemberUin);
                    await bot.SendGroupMessage(group.GroupUin, reply);
                    return;
                }
            }

            if (textChain is not null)
            {
                if (textChain.Content.Contains("可怜") && textChain.Content.Contains("我") &&
                    blackList.ContainsKey(group.MemberUin))
                {
                    await bot.GroupMuteMember(group.GroupUin, group.MemberUin, 360);
                    reply = new MessageBuilder();
                    reply.Text("你违反了lsp准则-xh专属规则，已被禁言6分钟");
                    reply.At(group.MemberUin);
                    await bot.SendGroupMessage(group.GroupUin, reply);
                    return;
                }
            }
            if (textChain is null)
            {
                return;
            }
            {
                var at = group.Chain.GetChain<AtChain>();
                if (at is not null && at.AtUin == bot.Uin)
                {

                    async Task IllustAsync(UserPreviewIllust[] illusts)
                    {
                        reply = new MessageBuilder();
                        if (!illusts.Any())
                        {
                            reply.Text("No illusts found.");
                        }
                        else
                        {
                            var ch = new MultiMsgChain();

                            var tsks = new List<Task>();

                            if (!whiteList.ContainsKey(group.GroupUin))
                            {
                                illusts = illusts.SkipWhile(x => x.Tags.Select(t => t.Name).Contains("R-18") || x.Tags.Select(t => t.Name).Contains("R-18G")).ToArray();
                            }
                            foreach (var item in illusts.Take(5))
                            {
                                async Task download()
                                {
                                    var re = new MessageBuilder();
                                    var bs = await Program.pixivAPI.DownloadBytesAsync(item.ImageUrls.Large.ToString());
                                    var ich = ImageChain.Create(bs);
                                    if (ich is null)
                                    {
                                        return;
                                    }
                                    re.Add(ich);
                                    re.Add(TextChain.Create($"\n标题：{item.Title}\n画师ID：{item.User.Id}\n图ID：{item.Id}"));
                                    lock (bot)
                                    {
                                        ch.AddMessage(bot.Uin, "寄", re.Build());
                                    }
                                }
                                tsks.Add(download());

                            }
                            await Task.WhenAll(tsks);

                            reply.Add(ch);
                        }
                    }


                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {

                            await Program.pixivAPI.AuthAsync("1WRRkxi2fNjvrY4ZcMFbyw5sOxnMf2uJojd5UjsCs7w");
                            Program.PixivHealthy = true;
                            break;
                        }
                        catch (System.Exception)
                        {
                            Program.PixivHealthy = false;
                        }
                    }

                    if (!Program.PixivHealthy)
                    {
                        reply = new MessageBuilder();

                        reply.Text("Pixiv API is not healthy, please contanct admin.");
                    }
                    else if (textChain.Content.Contains("原图"))
                    {
                        reply = new MessageBuilder();
                        var id = textChain.Content.Split("原图")[1].Trim();
                        var detail = await Program.pixivAPI.GetIllustDetailAsync(id);
                        var ch = new MultiMsgChain();
                        if (detail.Illust.MetaSinglePage.OriginalImageUrl is not null)
                        {
                            var url = detail.Illust.MetaSinglePage.OriginalImageUrl.ToString();
                            ch.AddMessage(bot.Uin, "寄", ImageChain.Create(await Program.pixivAPI.DownloadBytesAsync(url)));
                            reply.Add(ch);
                        }
                        else
                        {
                            for (int i = 0; i < detail.Illust.MetaPages.Length; i++)
                            {
                                var url = detail.Illust.MetaPages[i].ImageUrls.Original.ToString();
                                ch.AddMessage(bot.Uin, "寄", ImageChain.Create(await Program.pixivAPI.DownloadBytesAsync(url)));
                            }
                            reply.Add(ch);
                        }
                    }
                    else if (textChain.Content.Contains("搜索"))
                    {
                        var id = textChain.Content.Split("搜索")[1].Trim();
                        var rec = await Program.pixivAPI.GetSearchIllustAsync(id);
                        await IllustAsync(rec.Illusts);

                    }
                    else if (textChain.Content.Contains("日排行"))
                    {

                        var rec = await Program.pixivAPI.GetIllustRankingAsync("day_male");
                        await IllustAsync(rec.Illusts);

                    }
                    else if (textChain.Content.Contains("色图"))
                    {
                        if (whiteList.ContainsKey(group.GroupUin))
                        {
                            var rec = await Program.pixivAPI.GetIllustRecommendedAsync();
                            await IllustAsync(rec.Illusts.Where(x => x.Tags.Select(t => t.Name).Contains("R-18") || x.Tags.Select(t => t.Name).Contains("R-18G")).ToArray());
                        }
                        else
                        {
                            reply = new MessageBuilder();
                            reply.Text("h是不行的！");
                        }

                    }
                    else if (textChain.Content.Contains("图"))
                    {
                        if (whiteList.ContainsKey(group.GroupUin))
                        {
                            var rec = await Program.pixivAPI.GetIllustRecommendedAsync();
                            await IllustAsync(rec.Illusts);
                        }
                        else
                        {
                            reply = new MessageBuilder();
                            reply.Text("h是不行的！");
                        }

                    }
                    else if (textChain.Content.Contains("收藏"))
                    {
                        reply = new MessageBuilder();
                        var id = textChain.Content.Split("收藏")[1].Trim();
                        await Program.pixivAPI.PostIllustBookmarkAddAsync(id);
                        reply.Text("已收藏");
                    }
                }
                else if (textChain.Content.StartsWith("/help"))
                    reply = OnCommandHelp(textChain);
                if (textChain.Content.Contains("来首"))
                    reply = await OnKuwoAsync(textChain);
                else if (textChain.Content.StartsWith("/ping"))
                    reply = OnCommandPing(textChain);
                else if (textChain.Content.StartsWith("/status"))
                    reply = OnCommandStatus(textChain);
                else if (textChain.Content.StartsWith("/echo"))
                    reply = OnCommandEcho(textChain, group.Chain);
                else if (textChain.Content.StartsWith("/eval"))
                    reply = OnCommandEval(group.Chain);
                else if (textChain.Content.StartsWith("/member"))
                    reply = await OnCommandMemberInfo(bot, group);
                else if (textChain.Content.StartsWith("/mute"))
                    reply = await OnCommandMuteMember(bot, group);
                else if (textChain.Content.StartsWith("/title"))
                    reply = await OnCommandSetTitle(bot, group);
                else if (textChain.Content.StartsWith("/search"))
                {
                    if (!whiteList.ContainsKey(group.GroupUin))
                    {
                        reply = new MessageBuilder();
                        reply.Text("小朋友不许查");
                    }
                    else
                    {
                        reply = await Crawler.DemoSinglePageRequest(textChain);
                    }
                }
                else if (textChain.Content.StartsWith("BV"))
                    reply = await OnCommandBvParser(textChain);
                else if (textChain.Content.StartsWith("https://github.com/"))
                    reply = await OnCommandGithubParser(textChain);
                else if (Util.CanIDo(0.005))
                    reply = OnRepeat(group.Chain);
            }

            // Send reply message
            if (reply is not null)
            {
                var succ = false;
                for (int i = 0; i < 5; i++)
                {
                    var re = await bot.SendGroupMessage(group.GroupUin, reply);
                    if (re)
                    {
                        succ = true;
                        break;
                    }
                }
                if (!succ)
                {
                    reply = new();
                    reply.Text("Failed to send message.");
                    await bot.SendGroupMessage(group.GroupUin, reply);
                }

            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);

            // Send error print
            await bot.SendGroupMessage(group.GroupUin,
                Text($"{e.Message}\n{e.StackTrace}"));
        }
    }

    public static async Task<MessageBuilder> OnZood()
    {
        var mb = new MessageBuilder();

        var re = await Mp3ToRecChainAsync("https://lb-sycdn.kuwo.cn/8cf96adac93685f4b0cb05dcb60b692f/62d2afa7/resource/n1/86/45/3917959763.mp3");
        mb.Add(re);
        return mb;
    }

    public static async Task<MessageBuilder> OnKuwoAsync(TextChain chain)
    {
        var id = chain.Content.Split("来首")[1].Trim();
        return await GetMp3RecordAsync(id);
    }

    public static async Task<String?> GetNetEaseMusicAsync(string id)
    {
        var sre = await Program.neteaseAPI.RequestAsync(CloudMusicApiProviders.Cloudsearch, new Dictionary<string, object> { ["keywords"] = id }, false);
        var first = sre["result"]?["songs"]?[0]?["id"]?.ToString();
        if (first is null)
            return null;
        var ure = await Program.neteaseAPI.RequestAsync(CloudMusicApiProviders.SongUrlV1, new Dictionary<string, object> { ["id"] = first }, false);
        var url = ure["data"]?[0]?["url"]?.ToString();
        return url;
    }

    public static async Task<MessageBuilder> GetMp3RecordAsync(string id)
    {
        // var mb = new MessageBuilder();
        // var restr = await _client.GetStringAsync($"https://search.kuwo.cn/r.s?all={id}&ft=music&%20itemset=web_2013&client=kt&pn=0&rn=1&rformat=json&encoding=utf8");
        // var nstr = restr.Replace('\'', '"').Trim();
        // Console.WriteLine(nstr);
        // var re = JsonSerializer.Deserialize<KuwoSearchDto>(nstr)!;
        // if (re.abslist.Length == 0)
        // {
        //     mb.Text("找不到对应的歌");
        //     return mb;
        // }
        // var mp3url = await _client.GetStringAsync($"https://antiserver.kuwo.cn/anti.s?type=convert_url&rid={re.abslist[0].MUSICRID}&format=mp3&response=url");
        // mb.Add(await Mp3ToRecChainAsync(mp3url));
        // return mb;
        var mb = new MessageBuilder();
        var restr = await GetNetEaseMusicAsync(id);
        if (restr is null)
        {
            mb.Text("找不到对应的歌");
            return mb;
        }
        mb.Add(await Mp3ToRecChainAsync(restr));
        return mb;
    }
    public static async Task<RecordChain> Mp3ToRecChainAsync(string mp3url)
    {
        var id = Guid.NewGuid();
        using var mp3stream = await _client.GetStreamAsync(mp3url);
        var slkstream = new MemoryStream();

        // Create audio pipeline
        using var mp3pipeline = new AudioPipeline
        {
            // Mp3 decoder stream
            new  Mp3Codec.Decoder(mp3stream),
            new AudioResampler(new AudioInfo(AudioFormat.Signed16Bit, AudioChannel.Mono, 24000)),
            new SilkV3Codec.Encoder(),
            slkstream
        };
        var succ = await mp3pipeline.Start();
        if (!succ)
        {
            throw new Exception("slk encode failed");
        }
        Console.WriteLine($"slkstream length: {slkstream.Length}");
        slkstream.Position = 0;
        var re = RecordChain.Create(slkstream.GetBuffer()[..(int)slkstream.Length]);
        return re;
    }
    /// <summary>
    /// On help
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    public static MessageBuilder OnCommandHelp(TextChain chain)
    {
        var mb = new MessageBuilder()
            .Text("[帮助]\n")
            .Text("[at机器人] xxx图xxx\n 获取五张推荐图\n\n")
            .Text("[at机器人] 原图[id]\n 获取原图\n\n")
            .Text("[at机器人] 收藏[id]\n 收藏图\n\n")
            .Text("来首[歌]\n 放歌（语音形式）\n\n")
            .Text("/help\n 打印帮助\n\n")
            .Text("/member [at成员]\n 打印该成员信息\n\n")
            .Text("/mute [at成员] [时间（秒）]\n 禁言成员，默认60秒\n\n")
            .Text("/ping\n Pong!\n\n")
            .Text("/status\n Show bot status\n\n")
            .Text("/echo\n Send a message\n\n")
            .Text("/search [-c 0-6] [search text]\n 搜资源\n");
        mb.Text($"类型对应关系：\n");
        mb.Text($"\t0：不限制类型(默认为此参数)\n");
        foreach (var item in Crawler.Cat)
        {
            if (item.Key == 5)
            {
                continue;
            }
            mb.Text($"\t{item.Key}：{item.Value}\n");
        }
        mb.Text($"举例，搜索YOASOBI的专辑资源：/search -c 1 YOASOBI\n\n");
        return mb;
    }

    /// <summary>
    /// On status
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    public static MessageBuilder OnCommandStatus(TextChain chain)
        => new MessageBuilder()
            // Core descriptions
            .Text($"[Kagami]\n源码地址https://github.com/Chronostasys/Kagami\n")
            .Text($"[Konata.Core内核信息=======================]\n")
            .Text($"[branch:{BuildStamp.Branch}]\n")
            .Text($"[commit:{BuildStamp.CommitHash[..12]}]\n")
            .Text($"[version:{BuildStamp.Version}]\n")
            .Text($"[{BuildStamp.BuildTime}]\n\n")

            // System status
            .Text($"[===========系统信息=======================]\n")
            .Text($"Processed {_messageCounter} message(s)\n")
            .Text($"GC Allocated Memory {GC.GetTotalAllocatedBytes().Bytes2MiB(2)} MiB\n")
            .Text($"Total Memory {Process.GetCurrentProcess().WorkingSet64.Bytes2MiB(2)} MiB\n\n")

            .Text($"Is Pixiv Service Healthy: {Program.PixivHealthy} \n\n")

            // Copyrights
            .Text("Konata Project (C) 2022");

    /// <summary>
    /// On ping me
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    public static MessageBuilder OnCommandPing(TextChain? chain)
        => Text("Hello, I'm Kagami");

    /// <summary>
    /// On message echo <br/>
    /// <b>Safer than MessageBuilder.Eval()</b>
    /// </summary>
    /// <param name="text"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public static MessageBuilder OnCommandEcho(TextChain text, MessageChain chain)
        => new MessageBuilder(text.Content[5..].Trim()).Add(chain[1..]);

    /// <summary>
    /// On message eval
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    public static MessageBuilder OnCommandEval(MessageChain chain)
        => MessageBuilder.Eval(chain.ToString()[5..].TrimStart());

    /// <summary>
    /// On member info
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="group"></param>
    /// <returns></returns>
    public static async Task<MessageBuilder> OnCommandMemberInfo(Bot bot, GroupMessageEvent group)
    {
        // Get at
        var at = group.Chain.GetChain<AtChain>();
        if (at is null) return Text("Argument error");
        // Get group info
        var memberInfo = await bot.GetGroupMemberInfo(group.GroupUin, at.AtUin, true);
        if (memberInfo is null) return Text("No such member");

        return new MessageBuilder("[Member Info]\n")
            .Text($"Name: {memberInfo.Name}\n")
            .Text($"Join: {memberInfo.JoinTime}\n")
            .Text($"Role: {memberInfo.Role}\n")
            .Text($"Level: {memberInfo.Level}\n")
            .Text($"SpecTitle: {memberInfo.SpecialTitle}\n")
            .Text($"Nickname: {memberInfo.NickName}");
    }

    /// <summary>
    /// On mute
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="group"></param>
    /// <returns></returns>
    public static async Task<MessageBuilder> OnCommandMuteMember(Bot bot, GroupMessageEvent group)
    {
        // Get at
        var atChain = group.Chain.GetChain<AtChain>();
        if (atChain is null) return Text("Argument error");

        var time = 60U;
        var textChains = group.Chain
            .FindChain<TextChain>();
        {
            // Parse time
            if (textChains.Count is 2 &&
                uint.TryParse(textChains[1].Content, out var t))
            {
                time = t;
            }
        }
        if (atChain.AtUin == 1769712655)
        {
            return Text("不允许禁言开发者");
        }
        try
        {
            if (await bot.GroupMuteMember(group.GroupUin, atChain.AtUin, time))
                return Text($"Mute member [{atChain.AtUin}] for {time} sec.");
            return Text("Unknown error.");
        }
        catch (OperationFailedException e)
        {
            return Text($"{e.Message} ({e.HResult})");
        }
    }

    /// <summary>
    /// Set title
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="group"></param>
    /// <returns></returns>
    public static async Task<MessageBuilder> OnCommandSetTitle(Bot bot, GroupMessageEvent group)
    {
        // Get at
        var atChain = group.Chain.GetChain<AtChain>();
        if (atChain is null) return Text("Argument error");

        var textChains = group.Chain
            .FindChain<TextChain>();
        {
            // Check argument
            if (textChains.Count is not 2) return Text("Argument error");

            try
            {
                if (await bot.GroupSetSpecialTitle(group.GroupUin, atChain.AtUin, textChains[1].Content, uint.MaxValue))
                    return Text($"Set special title for member [{atChain.AtUin}].");
                return Text("Unknown error.");
            }
            catch (OperationFailedException e)
            {
                return Text($"{e.Message} ({e.HResult})");
            }
        }
    }

    /// <summary>
    /// Bv parser
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    public static async Task<MessageBuilder> OnCommandBvParser(TextChain chain)
    {
        var avCode = chain.Content.Bv2Av();
        if (avCode is "")
            return Text("Invalid BV code");
        var bytes = await $"https://www.bilibili.com/video/{avCode}".UrlDownload();

        // UrlDownload the page
        var html = Encoding.UTF8.GetString(bytes);

        // Get meta data
        var metaData = html.GetMetaData("itemprop");
        var titleMeta = metaData["name"];
        var descMeta = metaData["description"];
        var imageMeta = metaData["image"];
        var keyWdMeta = metaData["keywords"];

        // UrlDownload the image
        var image = await imageMeta.UrlDownload();

        // Build message
        var result = new MessageBuilder();
        {
            result.Text($"{titleMeta}\n");
            result.Text($"https://www.bilibili.com/video/{avCode}\n\n");
            result.Text($"{descMeta}\n");
            result.Image(image);
            result.Text("\n#" + string.Join(" #", keyWdMeta.Split(",")[1..^4]));
        }
        return result;
    }

    /// <summary>
    /// Github repo parser
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    public static async Task<MessageBuilder> OnCommandGithubParser(TextChain chain)
    {
        // UrlDownload the page
        try
        {
            var bytes = await $"{chain.Content.TrimEnd('/')}".UrlDownload();

            var html = Encoding.UTF8.GetString(bytes);

            // Get meta data
            var metaData = html.GetMetaData("property");
            var imageMeta = metaData["og:image"];

            // Build message
            var image = await imageMeta.UrlDownload();
            return new MessageBuilder().Image(image);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Not a repository link. \n" +
                              $"{e.Message}");
            return Text("Not a repository link.");
        }
    }

    /// <summary>
    /// Repeat
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static MessageBuilder OnRepeat(MessageChain message)
        => new(message);

    private static MessageBuilder Text(string text)
        => new MessageBuilder().Text(text);
}