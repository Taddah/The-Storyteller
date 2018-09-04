using System.IO;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace The_Storyteller.Entities.Tools
{
    internal class Config
    {
        [JsonProperty("color")] private string _color = "";

        [JsonProperty("prefix")] internal string Prefix = "";

        [JsonProperty("token")] internal string Token = "";

        internal DiscordColor Color => new DiscordColor(_color);


        public static Config LoadFromFile(string path)
        {
            using (var sr = new StreamReader(path))
            {
                return JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
            }
        }

        public void SaveToFile(string path)
        {
            using (var sw = new StreamWriter(path))
            {
                sw.Write(JsonConvert.SerializeObject(this));
            }
        }
    }
}