using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Gumi.Helpers
{
    public class Tag
    {
        public static bool Create(ulong owner, ulong guild, string name, string text)
        {
            JObject j = JObject.Parse(File.ReadAllText("tags.json"));
            if (j[name] == null)
            {
                j.Add(name, new JObject()
                {
                    {"owner", owner},
                    {"guild", guild},
                    {"text", text}
                });
                File.WriteAllText("tags.json", j.ToString());
                return true;
            }
            return false;
        }

        public static Tag Get(string name)
        {
            Tag result;
            JObject j = JObject.Parse(File.ReadAllText("tags.json"));
            if (j[name] != null)
            {
                JObject tag = (JObject)j[name];
                result = new Tag()
                {
                    exists = true,
                    name = name,
                    text = tag["text"].ToString(),
                    owner = ulong.Parse(tag["owner"].ToString()),
                    guild = ulong.Parse(tag["guild"].ToString())
                };
            }
            else
            {
                result = new Tag()
                {
                    exists = false
                };
            }
            return result;
        }

        public static void Remove()
        {

        }

        public bool exists;
        public string name;
        public string text;
        public ulong owner;
        public ulong guild;
    }
}
