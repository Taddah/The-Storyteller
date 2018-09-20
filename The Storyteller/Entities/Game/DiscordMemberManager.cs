using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace The_Storyteller.Entities.Game
{
    class DiscordMemberManager
    {
        private readonly List<DiscordMember> _discordMembers;
        private readonly string _filename;

        public DiscordMemberManager(string filename)
        {
            _filename = filename;
            _discordMembers = LoadFromFile();

            Task task = new Task(async () => await DoPeriodicCharacterSave());
            task.Start();
        }

        private List<DiscordMember> LoadFromFile()
        {
            if (!File.Exists(_filename))
            {
                return new List<DiscordMember>();
            }


            using (StreamReader sr = new StreamReader(_filename))
            {

                List<DiscordMember> res = JsonConvert.DeserializeObject<List<DiscordMember>>(sr.ReadToEnd());

                if (res != null)
                {
                    return res;
                }
                return new List<DiscordMember>();
            }
        }

        private async Task SaveToFile()
        {

            using (StreamWriter sw = new StreamWriter(_filename))
            {
                await sw.WriteAsync(JsonConvert.SerializeObject(_discordMembers));
            }
        }

        private async Task DoPeriodicCharacterSave()
        {
            while (true)
            {
                try
                {
                    await SaveToFile();
                }
                catch (IOException)
                {

                }

                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
        }

        public void StartAsyncSave()
        {
            Task task = new Task(async () => await SaveToFile());
            task.Start();
        }

        public void AddDiscordMember(DiscordMember m)
        {
            if (!IsPresent(m.Id))
            {
                _discordMembers.Add(m);
            }
        }

        public DiscordMember GetDiscordMemberById(ulong id)
        {
            return _discordMembers.SingleOrDefault(c => c.Id == id);
        }

        public bool IsPresent(ulong id)
        {
            return _discordMembers.Exists(x => x.Id == id);
        }
    }
}
