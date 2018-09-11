using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using The_Storyteller.Models;
using The_Storyteller.Models.MCharacter;

namespace The_Storyteller.Entities.Game
{
    internal class CharacterManager
    {
        private readonly List<Character> _characters;
        private readonly string _filename;

        public CharacterManager(string filename)
        {
            _filename = filename;
            _characters = LoadFromFile();

            Task task = new Task(async () => await DoPeriodicCharacterSave());
            task.Start();
        }

        private List<Character> LoadFromFile()
        {
            if (!File.Exists(_filename))
                return new List<Character>();
            using (var sr = new StreamReader(_filename))
            {
                var res = JsonConvert.DeserializeObject<List<Character>>(sr.ReadToEnd());
                if (res != null) return res;
                return new List<Character>();
            }
        }

        private async Task SaveToFile()
        {
            using (var sw = new StreamWriter(_filename))
            {
                await sw.WriteAsync(JsonConvert.SerializeObject(_characters));
            }
        }

        public async Task DoPeriodicCharacterSave()
        {
            while (true)
            {
                await SaveToFile();
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
        }

        public void StartAsyncSave()
        {
            Task task = new Task(async () => await SaveToFile());
            task.Start();
        }

        public void AddCharacter(Character c)
        {
            if (!IsPresent(c.DiscordID))
            {
                _characters.Add(c);
                StartAsyncSave();
            }
        }

        public void EditCharacter(Character c)
        {
            if (IsPresent(c.DiscordID))
            {
                var oldC = GetCharacterByDiscordId(c.DiscordID);
                oldC = c;
                StartAsyncSave();
            }
        }

        public Character GetCharacterByDiscordId(ulong discordId)
        {
            return _characters.SingleOrDefault(x => x.DiscordID == discordId);
        }

        public Character GetCharacterById(int id)
        {
            return _characters.SingleOrDefault(x => x.Id == id);
        }

        public Character GetCharacterByTrueName(string trueName)
        {
            return _characters.SingleOrDefault(c => c.TrueName.ToLower() == trueName.ToLower());
        }

        public bool IsPresent(ulong id)
        {
            return _characters.Exists(x => x.DiscordID == id);
        }

        public bool IsTrueNameTaken(string surname)
        {
            return _characters.Exists(c => c.TrueName.ToLower() == surname.ToLower());
        }

        public int GetCount()
        {
            return _characters.Count();
        }

        public void DeleteCharacter(ulong discordId)
        {
            var delC = _characters.SingleOrDefault(c => c.DiscordID == discordId);
            if (delC != null) _characters.Remove(delC);
        }
    }
}