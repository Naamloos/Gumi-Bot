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
        public static bool Create(ulong owner, ulong guild, string name, string text, string attachment)
        {
            JObject j = JObject.Parse(File.ReadAllText("tags.json"));
            if (j[name] == null || j[name][owner].ToString() == owner.ToString())
            {
                Remove(name, owner);
                j.Add(name, new JObject()
                {
                    {"owner", owner},
                    {"guild", guild},
                    {"text", text},
                    { "attachment", attachment }
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
                    guild = ulong.Parse(tag["guild"].ToString()),
                    attachment = tag["attachment"].ToString()
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

        public static List<string> List()
        {
            List<string> taglist = new List<string>();
            JObject j = JObject.Parse(File.ReadAllText("tags.json"));
            foreach(var t in j)
            {
                taglist.Add(t.Key);
            }
            return taglist;
        }

        public static bool Remove(string name, ulong user)
        {
            JObject j = JObject.Parse(File.ReadAllText("tags.json"));
            if (j[name] != null)
            {
                if(j[name]["owner"].ToString() == user.ToString())
                {
                    j.Remove(name);
                    File.WriteAllText("tags.json", j.ToString());
                    return true;
                }
            }
            return false;
        }

        public bool exists;
        public string name;
        public string text;
        public string attachment;
        public ulong owner;
        public ulong guild;
    }
}
