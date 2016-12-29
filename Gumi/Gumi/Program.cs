using DSharpPlus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gumi
{
    class Program
    {
        public static string prefix = "||";
        static void Main(string[] args)
        {
            if (!File.Exists("token.txt"))
            {
                File.Create("token.txt").Close();
            }
            if(File.ReadAllText("token.txt") == "")
            {
                Console.WriteLine("Please enter your bot's token in token.txt.");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            string token = File.ReadAllText("token.txt");

            DiscordClient _client = new DiscordClient(new DiscordConfig()
            {
                AutoReconnect = true,
                DiscordBranch = Branch.Canary,
                Token = token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Unnecessary,
                VoiceSettings = VoiceSettings.None
            });

            _client.Ready += async (sender, e) =>
            {
                await _client.UpdateStatus("💕 " + prefix + "help 💕");
                _client.DebugLogger.LogMessage(LogLevel.Info, "Gumi", "Ready!", DateTime.UtcNow);
            };

            _client.MessageCreated += async (sender, e) =>
            {
                if (e.Message.Content.StartsWith(prefix))
                {
                    string command = e.Message.Content.Substring(prefix.Length);
                    if(command == "ping")
                    {
                        await e.Message.Respond("pong!");
                    }
                    else if(command == "pong")
                    {
                        await e.Message.Respond("ping!");
                    }else if(command == "info")
                    {
                        DiscordEmbed embed = new DiscordEmbed()
                        {
                            Title = "Gumi on Github",
                            Description = "Gumi is a Discord bot written in C#,\nRunning DSharpPlus rewrite.",
                            Author = new DiscordEmbedAuthor()
                            {
                                Name = "Written by Naamloos",
                                Url = "https://www.discord.gg/0oZpaYcAjfvkDuE4",
                                IconUrl = "https://cdn.discordapp.com/attachments/146044397861994496/264049233487724545/127408598010560513.png"
                            },
                            Color = 1,
                            Url = "https://github.com/NaamloosDT/Gumi-Bot",
                            Thumbnail = new DiscordEmbedThumbnail()
                            {
                                Url = _client.Me.AvatarUrl
                            }
                        };
                        await e.Message.Respond("", embed: embed);
                    }
                }
            };

            _client.Connect();
            Console.ReadKey();
        }
    }
}
