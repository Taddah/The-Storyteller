using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using The_Storyteller.Models.MVillage;

namespace The_Storyteller.Entities.Game
{
    /// <summary>
    /// Gère les villages et la sauvegarde dans un fichier json
    /// </summary>
    class VillageManager
    {
        private readonly List<Village> _villages;
        private readonly string _filename;

        public VillageManager(string filename)
        {
            _filename = filename;
            _villages = LoadFromFile();

            Task task = new Task(async () => await DoPeriodicCharacterSave());
            task.Start();
        }

        private List<Village> LoadFromFile()
        {
            if (!File.Exists(_filename))
                return new List<Village>();
            using (var sr = new StreamReader(_filename))
            {
                var res = JsonConvert.DeserializeObject<List<Village>>(sr.ReadToEnd());
                if (res != null) return res;
                return new List<Village>();
            }
        }

        private async Task SaveToFile()
        {
            try
            {
                using (var sw = new StreamWriter(_filename))
                {
                    await sw.WriteAsync(JsonConvert.SerializeObject(_villages));
                }
            }
            catch (IOException)
            {

            }
           
        }

        private async Task DoPeriodicCharacterSave()
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

        public void AddVillage(Village v)
        {
            if (!Exists(v.Id))
            {
                _villages.Add(v);
                StartAsyncSave();
            }
        }

        public void EditVillage(Village v)
        {
            if (Exists(v.Id))
            {
                var oldVillage = GetVillageById(v.Id);
                oldVillage = v;
                StartAsyncSave();
            }
        }

        public Village GetVillageById(int id)
        {
            return _villages.SingleOrDefault(v => v.Id == id);
        }

        internal Village GetVillageByName(string villageName)
        {
            return _villages.SingleOrDefault(v => v.Name.ToLower() == villageName.ToLower());
        }

        public bool Exists(int id)
        {
            return _villages.Exists(v => v.Id == id);
        }

        public bool IsNameTaken(string name)
        {
            return _villages.Exists(v => v.Name == name);
        }

        public int GetVillageCount()
        {
            return _villages.Count;
        }

    }
}
