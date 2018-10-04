using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
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

            List<Character> listCha = new List<Character>();

            XmlDocument doc = new XmlDocument();
            doc.Load(_filename);

            //get inventories
            XmlNode characters = doc.GetElementsByTagName("characters").Item(0);

            //Pour chaque inventaire
            foreach (XmlElement character in characters.ChildNodes)
            {
                if (!ulong.TryParse(character.GetAttribute("id"), out ulong id))
                {
                    //Pas d'id, on stop le chargement de ce personnage
                    break;
                }

                int.TryParse(character.GetAttribute("level"), out int level);
                int.TryParse(character.GetAttribute("experience"), out int experience);
                int.TryParse(character.GetAttribute("energy"), out int energy);
                int.TryParse(character.GetAttribute("maxEnergy"), out int maxEnergy);
                int.TryParse(character.GetAttribute("locationX"), out int locationX);
                int.TryParse(character.GetAttribute("locationY"), out int locationY);


                Character cha = new Character
                {
                    Id = id,
                    Energy = energy,
                    MaxEnergy = maxEnergy,
                    Level = level,
                    Experience = experience,
                    Location = new Models.MMap.Location(locationX, locationY),
                    Name = character.GetAttribute("name"),
                    OriginRegionName = character.GetAttribute("originRegionName"),
                    Profession = (Profession)Enum.Parse(typeof(Profession), character.GetAttribute("profession")),
                    Sex = (Sex)Enum.Parse(typeof(Sex), character.GetAttribute("sex")),
                    TrueName = character.GetAttribute("trueName"),
                    VillageName = character.GetAttribute("villageName"),
                    Stats = CharacterStats.Deserialize((XmlElement)character.ChildNodes[0]),
                    Skills = CharacterSkills.Deserialize((XmlElement)character.ChildNodes[1])
                };
                listCha.Add(cha);
            }
            
            return listCha;
        }

        private async Task SaveToFile()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("characters");

            foreach (Character c in _characters)
            {
                XmlElement xmlCharacter = doc.CreateElement("character");

                //General information
                xmlCharacter.SetAttribute("id", c.Id.ToString());
                xmlCharacter.SetAttribute("name", c.Name);
                xmlCharacter.SetAttribute("trueName", c.TrueName);
                xmlCharacter.SetAttribute("sex", c.Sex.ToString());
                xmlCharacter.SetAttribute("level", c.Level.ToString());
                xmlCharacter.SetAttribute("experience", c.Experience.ToString());
                xmlCharacter.SetAttribute("energy", c.Energy.ToString());
                xmlCharacter.SetAttribute("maxEnergy", c.MaxEnergy.ToString());
                xmlCharacter.SetAttribute("locationX", c.Location.XPosition.ToString());
                xmlCharacter.SetAttribute("locationY", c.Location.YPosition.ToString());
                xmlCharacter.SetAttribute("origonRegionName", c.OriginRegionName);
                xmlCharacter.SetAttribute("villageName", c.VillageName);
                xmlCharacter.SetAttribute("profession", c.Profession.ToString());

                //Character stats
                xmlCharacter.AppendChild(c.Stats.Serialize(doc));

                //Character skills
                XmlElement xmlSkills = doc.CreateElement("skills");
                foreach (CharacterSkills cs in c.Skills)
                {
                    XmlElement xmlCs = doc.CreateElement("skill");
                    xmlCs.SetAttribute("name", cs.Name);
                    xmlCs.SetAttribute("level", cs.Level.ToString());
                    xmlCs.SetAttribute("experience", cs.Experience.ToString());
                    xmlSkills.AppendChild(xmlCs);
                }
                xmlCharacter.AppendChild(xmlSkills);

                root.AppendChild(xmlCharacter);
            }

            doc.AppendChild(root);

            await Task.Factory.StartNew(delegate
            {
                using (StreamWriter sw = new StreamWriter(_filename))
                {
                    doc.Save(sw);
                }
            });
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