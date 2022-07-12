using Kagami.Function;
using Konata.Core;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces;
using Konata.Core.Interfaces.Api;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Kagami;

public static class Program
{
    private static Bot _bot = null!;
    internal static PixivCS.PixivAppAPI pixivAPI = new();

    public static async Task Main()
    {
        var token = await File.ReadAllTextAsync("pixiv.refreshtoken");
        Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    await pixivAPI.AuthAsync(token);
                    await File.WriteAllTextAsync("pixiv.refreshtoken",pixivAPI.RefreshToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                await Task.Delay(1200);
            }
        });
        _bot = BotFather.Create(GetConfig(),
            GetDevice(), GetKeyStore());
        {
            // Print the log
            _bot.OnLog += (_, e) => Console.WriteLine(e.EventMessage);

            // Handle the captcha
            _bot.OnCaptcha += (s, e) =>
            {
                switch (e.Type)
                {
                    case CaptchaEvent.CaptchaType.Sms:
                        Console.WriteLine(e.Phone);
                        s.SubmitSmsCode(Console.ReadLine());
                        break;

                    case CaptchaEvent.CaptchaType.Slider:
                        Console.WriteLine(e.SliderUrl);
                        var ticket = Console.ReadLine();
                        s.SubmitSliderTicket(ticket);
                        break;

                    default:
                    case CaptchaEvent.CaptchaType.Unknown:
                        break;
                }
            };

            // Handle poke messages
            _bot.OnGroupPoke += Poke.OnGroupPoke;

            // Handle messages from group
            _bot.OnGroupMessage += Command.OnGroupMessage;
        }

        // Login the bot
        var result = await _bot.Login();
        {
            // Update the keystore
            if (result) UpdateKeystore(_bot.KeyStore);
        }

        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        Thread.Sleep(Timeout.Infinite);
    }

    private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
    {
        _bot.Logout().Wait();
        _bot.Dispose();
    }

    /// <summary>
    /// Get bot config
    /// </summary>
    /// <returns></returns>
    private static BotConfig GetConfig()
    {
        return new BotConfig
        {
            EnableAudio = true,
            TryReconnect = true,
            HighwayChunkSize = 8192,
        };
    }

    /// <summary>
    /// Load or create device 
    /// </summary>
    /// <returns></returns>
    private static BotDevice? GetDevice()
    {
        // Read the device from config
        if (File.Exists("device.json"))
        {
            return JsonSerializer.Deserialize
                <BotDevice>(File.ReadAllText("device.json"));
        }

        // Create new one
        var device = BotDevice.Default();
        {
            var deviceJson = JsonSerializer.Serialize(device,
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("device.json", deviceJson);
        }

        return device;
    }

    /// <summary>
    /// Load or create keystore
    /// </summary>
    /// <returns></returns>
    private static BotKeyStore? GetKeyStore()
    {
        // Read the device from config
        if (File.Exists("keystore.json"))
        {
            return JsonSerializer.Deserialize
                <BotKeyStore>(File.ReadAllText("keystore.json"));
        }

        Console.WriteLine("For first running, please " +
                          "type your account and password.");

        Console.Write("Account: ");
        var account = Console.ReadLine();

        Console.Write("Password: ");
        var password = Console.ReadLine();

        // Create new one
        Console.WriteLine("Bot created.");
        return UpdateKeystore(new BotKeyStore(account, password));
    }

    /// <summary>
    /// Update keystore
    /// </summary>
    /// <param name="keystore"></param>
    /// <returns></returns>
    private static BotKeyStore UpdateKeystore(BotKeyStore keystore)
    {
        var deviceJson = JsonSerializer.Serialize(keystore,
            new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("keystore.json", deviceJson);
        return keystore;
    }
}
