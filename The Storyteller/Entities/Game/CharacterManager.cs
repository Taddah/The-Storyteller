using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using The_Storyteller.Models.MCharacter;

namespace The_Storyteller.Entities.Game
{
    /// <summary>
    /// Gère les Character depuis une liste
    /// Sauvegarde automatique dans un json toutes les 10 secondes
    /// </summary>
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
            {
                return new List<Character>();
            }
            

            using (StreamReader sr = new StreamReader(_filename))
            {

                List<Character> res = JsonConvert.DeserializeObject<List<Character>>(sr.ReadToEnd());

                if (res != null)
                {
                    return res;
                }
                return new List<Character>();
            }
        }

        private async Task SaveToFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Character>));

            using (StreamWriter sw = new StreamWriter(_filename))
            {
                JsonSerializerSettings jset = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
                await sw.WriteAsync(JsonConvert.SerializeObject(_characters));
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

        public void AddCharacter(Character c)
        {
            if (!IsPresent(c.Id))
            {
                _characters.Add(c);
            }
        }

        public void EditCharacter(Character c)
        {
            if (IsPresent(c.Id))
            {
                Character oldC = GetCharacterByDiscordId(c.Id);
                oldC = c;
            }
        }

        public Character GetCharacterByDiscordId(ulong discordId)
        {
            return _characters.SingleOrDefault(x => x.Id == discordId);
        }


        public Character GetCharacterByTrueName(string trueName)
        {
            return _characters.SingleOrDefault(c => c.TrueName.ToLower() == trueName.ToLower());
        }

        public Character GetCharacterByName(string name)
        {
            return _characters.SingleOrDefault(c => c.Name.ToLower() == name.ToLower());
        }

        public bool IsPresent(ulong id)
        {
            return _characters.Exists(x => x.Id == id);
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
            Character delC = _characters.SingleOrDefault(c => c.Id == discordId);
            if (delC != null)
            {
                _characters.Remove(delC);
            }
        }
    }
}