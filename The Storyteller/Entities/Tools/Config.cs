using System.IO;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace The_Storyteller.Entities.Tools
{
    internal class Config
    {
        [JsonProperty("color")] private readonly string _color = "";

        [JsonProperty("prefix")] internal string Prefix = "";

        [JsonProperty("token")] internal string Token = "";

        internal DiscordColor Color => new DiscordColor(_color);

        private static Config instance = null;
        private static readonly object padlock = new object();

        Config()
        {
        }

        public static Config Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Config();
                    }
                    return instance;
                }
            }
        }


        public void LoadFromFile(string path)
        {
            using (var sr = new StreamReader(path))
            {
                instance =  JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
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