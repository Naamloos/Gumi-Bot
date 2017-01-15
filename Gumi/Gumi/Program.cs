using DSharpPlus;
using Gumi.Helpers;
using Newtonsoft.Json.Linq;
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
        public static List<string> blacklist;
        public static DateTime start;
        static void Main(string[] args)
        {
            start = DateTime.Now;
            // These commands shouldn't get tags.
            blacklist = new List<string>()
            {
                "ping",
                "pong",
                "tag",
                "info",
                "guild"
            };

            if (!File.Exists("token.txt"))
            {
                File.Create("token.txt").Close();
            }
            if (File.ReadAllText("token.txt") == "")
            {
                Console.WriteLine("Please enter your bot's token in token.txt.");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            if (!File.Exists("tags.json"))
            {
                File.Create("tags.json").Close();
                File.WriteAllText("tags.json", "{ }");
            }
            if (!File.Exists("trusted.json"))
            {
                JArray trusted = new JArray()
                {
                    127408598010560513,
                    227057052361555968
                };
                File.Create("trusted.json").Close();
                File.WriteAllText("trusted.json", trusted.ToString());
            }
            if (!File.Exists("blocked.json"))
            {
                JArray blocked = new JArray()
                {
                    0
                };
                File.Create("blocked.json").Close();
                File.WriteAllText("blocked.json", blocked.ToString());
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
                _client.DebugLogger.LogMessage(LogLevel.Debug, "Gumi-Debug", "Ready!", DateTime.UtcNow);
            };

            _client.MessageCreated += async (sender, e) =>
            {
                _client.DebugLogger.LogMessage(LogLevel.Info, "Gumi-Chat", e.Message.Author.Username + "#" + e.Message.Author.Discriminator + "@" + e.Guild.Name + "#"
                    + e.Channel.Name + ": " + e.Message.Content, DateTime.UtcNow);
                if (e.Message.Content.StartsWith(prefix))
                {
                    string command = e.Message.Content.Substring(prefix.Length);
                    if (command == "ping")
                    {
                        await e.Message.Respond("🎶 pong! 🎶");
                    }
                    else if (command == "pong")
                    {
                        await e.Message.Respond("🎶 ping! 🎶");
                    }
                    else if (command == "info")
                    {
                        DiscordEmbed embed = new DiscordEmbed()
                        {
                            Title = "🎶 Gumi on Github 🎶",
                            Description = "Gumi is a Discord bot written in C#,\nRunning DSharpPlus.",
                            Author = new DiscordEmbedAuthor()
                            {
                                Name = "Written by Naamloos ❤",
                                Url = "https://www.discord.gg/0oZpaYcAjfvkDuE4",
                                IconUrl = "https://cdn.discordapp.com/attachments/146044397861994496/264049233487724545/127408598010560513.png"
                            },
                            Color = 65430,
                            Url = "https://github.com/NaamloosDT/Gumi-Bot",
                            Thumbnail = new DiscordEmbedThumbnail()
                            {
                                Url = _client.Me.AvatarUrl
                            }
                        };
                        await e.Message.Respond("", embed: embed);
                    }
                    else if (command == "guild")
                    {
                        DiscordGuild g = e.Guild;
                        DiscordUser o = await _client.GetUser(e.Guild.OwnerID.ToString());
                        string info = "🎶 **Guild info for *\"" + g.Name + "\"*.** 🎶";
                        info += "\n```";
                        info += "\nChannel count: " + g.Channels.Count;
                        info += "\nMember count: " + g.MemberCount;
                        info += "\nCreated: " + g.CreationDate.ToString();
                        info += "\nAmount of custom emojis: " + g.Emojis.Count;
                        info += "\nMy join date: " + g.JoinedAt.ToString();
                        info += "\nRole count: " + g.Roles.Count;
                        info += "\nGuild owner: " + o.Username + "#" + o.Discriminator;
                        info += "\nGuild owner ID: " + o.ID;
                        info += "\n```";
                        info += "\nIcon URL: " + g.IconUrl;
                        await e.Message.Respond(info);
                    }
                    else if (command == "uptime")
                    {
                        await e.Channel.SendMessage($"🎶 My uptime: **{DateTime.Now.Subtract(start).ToString()}** 🎶");
                    }
                    else if (command == "tag")
                    {
                        string tags = "🎶 **List of tags:** 🎶\n```";
                        foreach (string tag in Tag.List())
                        {
                            tags += tag + ", ";
                        }
                        tags += "```";
                        await e.Message.Respond("🎶 Sent a DM with all tags! 🎶");
                        DiscordChannel dm = await _client.CreateDM(e.Message.Author.ID);
                        await dm.SendMessage(tags);
                    }
                    else if (command.StartsWith("tag "))
                    {
                        // I call this: if-statement frankenstein!
                        string name = command.Substring(4);
                        if (name.StartsWith("create "))
                        {
                            if (name.Length > 7)
                            {
                                string newname = name.Substring(7).Split(' ')[0].ToLower();
                                if (name.Substring(7 + newname.Length) != "")
                                {
                                    string text = name.Substring(7 + newname.Length + 1);
                                    string attachment = "";
                                    if (e.Message.Attachments.Count > 0)
                                    {
                                        attachment = e.Message.Attachments.First().Url;
                                    }
                                    if (Tag.Create(e.Message.Author.ID, e.Guild.ID, newname, text, attachment))
                                    {
                                        await e.Message.Respond("🎶 Tag created! (" + newname + "). 🎶");
                                        _client.DebugLogger.LogMessage(LogLevel.Info, "Gumi-Chat", "Created a tag: " + newname, DateTime.UtcNow);
                                    }
                                    else
                                    {
                                        await e.Message.Respond("🎶 Tag already exists! (" + newname + "). 🎶");
                                    }
                                }
                                else
                                {
                                    await e.Message.Respond("🎶 Invalid arguments! 🎶");
                                }
                            }
                            else
                            {
                                await e.Message.Respond("🎶 Invalid arguments! 🎶");
                            }
                        }
                        else if (name.StartsWith("delete "))
                        {
                            string deletename = name.Substring(7);
                            if (Tag.Remove(deletename, e.Message.Author.ID))
                                await e.Message.Respond("🎶 Deleted tag: " + deletename + " 🎶");
                            else
                                await e.Message.Respond("🎶 Can't delete tag \"" + deletename + "\", it either doesn't exist or you don't own it! 🎶");
                        }
                    }
                    else
                    {
                        if (Tag.List().Contains(command.ToLower()))
                        {
                            Tag t = Tag.Get(command.ToLower());
                            DiscordUser owner = await _client.GetUser(t.owner.ToString());
                            DiscordEmbed embed = new DiscordEmbed()
                            {
                                Title = "**Tag name: " + t.name + "**",
                                Description = t.text,
                                Author = new DiscordEmbedAuthor()
                                {
                                    Name = "Author: " + owner.Username + "#" + owner.Discriminator,
                                    IconUrl = owner.AvatarUrl
                                },
                            };
                            embed.Image = new DiscordEmbedImage()
                            {
                                Url = t.attachment
                            };
                            await e.Message.Respond("", embed: embed);
                        }
                    }
                }

            };

            _client.Connect(0);
            Console.ReadKey();
        }
    }
}
