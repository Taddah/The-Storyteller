using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        }

        public List<Character> LoadFromFile()
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

        public void SaveToFile()
        {
            using (var sw = new StreamWriter(_filename))
            {
                sw.Write(JsonConvert.SerializeObject(_characters));
            }
        }

        public void AddCharacter(Character c)
        {
            if (!IsPresent(c.Id))
            {
                _characters.Add(c);
                SaveToFile();
            }
        }

        public void EditCharacter(Character c)
        {
            if (IsPresent(c.Id))
            {
                var oldC = GetCharacterById(c.Id);
                oldC = c;
                SaveToFile();
            }
        }

        public Character GetCharacterById(ulong id)
        {
            return _characters.SingleOrDefault(x => x.Id == id);
        }

        public Character GetCharacterByTrueName(string trueName)
        {
            var ch = _characters.SingleOrDefault(c => c.TrueName == trueName);

            return ch;
        }

        public bool IsPresent(ulong id)
        {
            return _characters.Exists(x => x.Id == id);
        }

        public bool IsTrueNameTaken(string surname)
        {
            return _characters.Exists(c => c.TrueName.ToLower() == surname.ToLower());
        }
    }
}