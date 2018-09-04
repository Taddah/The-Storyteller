using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using The_Storyteller.Models;

namespace The_Storyteller.Entities.Game
{
    internal class GuildManager
    {
        private readonly string _filename;

        [JsonProperty("guilds")] private List<Guild> _guilds;

        public GuildManager(string filename)
        {
            _filename = filename;
            _guilds = LoadFromFile();
        }

        public List<Guild> LoadFromFile()
        {
            if (!File.Exists(_filename))
                return new List<Guild>();
            using (var sr = new StreamReader(_filename))
            {
                var res = JsonConvert.DeserializeObject<List<Guild>>(sr.ReadToEnd());
                if (res != null) return res;
                return new List<Guild>();
            }
        }

        public void SaveToFile()
        {
            using (var sw = new StreamWriter(_filename))
            {
                sw.Write(JsonConvert.SerializeObject(_guilds));
            }
        }

        public void AddGuild(Guild g)
        {
            if (!IsPresent(g.Id))
            {
                _guilds.Add(g);
                SaveToFile();
            }
        }

        public Guild GetGuildById(ulong id)
        {
            return _guilds.Find(x => x.Id == id);
        }

        public bool IsPresent(ulong id)
        {
            return _guilds.Exists(x => x.Id == id);
        }
    }
}