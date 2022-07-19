using Kagami.Utils;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Exceptions.Model;
using Konata.Core.Interfaces.Api;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedParameter.Local

namespace Kagami.Function;

public static class Command
{
    private static uint _messageCounter;
    private static HttpClient _client = new();
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
        if (textChain is null) return;
        try
        {
            MessageBuilder? reply = null;
            {
                var at = group.Chain.GetChain<AtChain>();
                if (at is not null && at.AtUin == bot.Uin)
                {

                    if (!Program.PixivHealthy)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            try
                            {
                                await Program.pixivAPI.AuthAsync(await File.ReadAllTextAsync("pixiv.refreshtoken"));
                                Program.PixivHealthy = true;
                                break;
                            }
                            catch (System.Exception)
                            {
                            }
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
                        var url = detail.Illust.MetaSinglePage.OriginalImageUrl.ToString();
                        var ch = new MultiMsgChain();
                        ch.AddMessage(bot.Uin, "寄", ImageChain.Create(await Program.pixivAPI.DownloadBytesAsync(url)));
                        reply.Add(ch);
                    }
                    else if (textChain.Content.Contains("图"))
                    {

                        var rec = await Program.pixivAPI.GetIllustRecommendedAsync();
                        reply = new MessageBuilder();
                        if (!rec.Illusts.Any())
                        {
                            reply.Text("No illusts found.");
                        }
                        else
                        {
                            var ch = new MultiMsgChain();

                            var tsks = new List<Task>();
                            foreach (var item in rec.Illusts.Take(5))
                            {
                                async Task download()
                                {
                                    var re = new MessageBuilder();
                                    var bs = await Program.pixivAPI.DownloadBytesAsync(item.ImageUrls.Large.ToString());
                                    re.Add(ImageChain.Create(bs));
                                    re.Add(TextChain.Create($"\n标题：{item.Title}\n画师ID：{item.User.Id}\n图ID：{item.Id}"));
                                    var succ = await bot.SendFriendMessage(2293738051, re);
                                    if (succ)
                                    {
                                        ch.Add(new MessageStruct(bot.Uin, "寄", re.Build()));
                                    }
                                    else
                                    {
                                        ch.AddMessage(bot.Uin, "寄", TextChain.Create($"此图无法上传\n标题：{item.Title}\n画师ID：{item.User.Id}\n图ID：{item.Id}"));
                                    }
                                }
                                tsks.Add(download());

                            }
                            await Task.WhenAll(tsks);

                            reply.Add(ch);
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
                else if (textChain.Content.StartsWith("小溜一首"))
                    reply = await OnZood();
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
                    reply = await Crawler.DemoSinglePageRequest(textChain);
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
        var mb = new MessageBuilder();
        var id = chain.Content.Split("来首")[1].Trim();
        var restr = await _client.GetStringAsync($"https://search.kuwo.cn/r.s?all={id}&ft=music&%20itemset=web_2013&client=kt&pn=0&rn=1&rformat=json&encoding=utf8");
        var nstr = restr.Replace('\'', '"').Trim();
        Console.WriteLine(nstr);
        var re = JsonSerializer.Deserialize<KuwoSearchDto>(nstr);
        if (re.abslist.Length==0)
        {
            mb.Text("找不到对应的歌");
            return mb;
        }
        var mp3url = await _client.GetStringAsync($"https://antiserver.kuwo.cn/anti.s?type=convert_url&rid={re.abslist[0].MUSICRID}&format=mp3&response=url");
        mb.Add(await Mp3ToRecChainAsync(mp3url));
        return mb;
    }
    public static async Task<RecordChain> Mp3ToRecChainAsync(string mp3url)
    {
        var id = Guid.NewGuid();
        var mp3stream = await _client.GetStreamAsync(mp3url);
        var fs = File.Open($"/root/{id}.mp3", FileMode.Create);
        await mp3stream.CopyToAsync(fs);
        await mp3stream.DisposeAsync();
        await fs.DisposeAsync();
        await Process.Start("/snap/bin/ffmpeg", $"-y  -i /root/{id}.mp3  -acodec pcm_s16le -f s16le -ac 1 -ar 24000 /root/{id}.pcm").WaitForExitAsync();
        await Process.Start("/root/silk-v3-decoder/silk/encoder", $"/root/{id}.pcm /root/{id}.silk").WaitForExitAsync();
        var re = RecordChain.Create(await File.ReadAllBytesAsync($"/root/{id}.silk"));
        File.Delete($"{id}.mp3");
        File.Delete($"{id}.pcm");
        File.Delete($"{id}.silk");
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
            .Text("/mute [时间（秒）] [at成员]\n 禁言成员，默认60秒\n\n")
            .Text("/ping\n Pong!\n\n")
            .Text("/status\n Show bot status\n\n")
            .Text("/echo\n Send a message\n\n")
            .Text("/search [-c 0-6] [search text]\n 搜资源\n");
        mb.Text($"类型对应关系：\n");
        mb.Text($"\t0：不限制类型(默认为此参数)\n");
        foreach (var item in Crawler.Cat)
        {
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
            var bytes = await $"{chain.Content.TrimEnd('/')}.git".UrlDownload();

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