using System.Collections.Generic;
using System.Xml;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MGameObject.Resources;

namespace The_Storyteller.Models.MMap
{

    internal abstract class Case
    {
        public Location Location { get; set; }
        public bool IsAvailable { get; set; }
        public ulong VillageId { get; set; }
        public List<Resource> Resources { get; set; }

        //Character présent sur la map
        public List<ulong> CharactersPresent { get; set; }

        public Case()
        {
            CharactersPresent = new List<ulong>();
            Resources = new List<Resource>();
            GenerateResources();
        }

        /// <summary>
        /// Case constructible pour un village
        /// if ((Location.XPosition + Location.YPosition) % 9 != 0) return false;
        /// </summary>
        /// <returns></returns>
        public abstract bool IsBuildable();

        /// <summary>
        /// Generate resources
        /// </summary>
        public abstract void GenerateResources();

        public abstract string GetTypeOfCase();

        public XmlElement Serialize(XmlDocument doc)
        {
            XmlElement element = doc.CreateElement("case");
            element.SetAttribute("type", GetTypeOfCase().ToLower());
            element.SetAttribute("locationX", Location.XPosition.ToString());
            element.SetAttribute("locationY", Location.YPosition.ToString());
            element.SetAttribute("isAvailable", this.IsAvailable.ToString());
            element.SetAttribute("villageId", this.VillageId.ToString());

            XmlElement resources = doc.CreateElement("resources");
            foreach(Resource r in Resources)
            {
                resources.AppendChild(r.Serialize(doc));
            }

            XmlElement characters = doc.CreateElement("characters");
            foreach (ulong charId in CharactersPresent)
            {
                XmlElement charXml = doc.CreateElement(charId.ToString());
                resources.AppendChild(charXml);
            }

            element.AppendChild(resources);
            element.AppendChild(characters);
            return element;

        }

        public void AddNewCharacter(Character c)
        {
            if (CharactersPresent == null)
            {
                return;
            }

            CharactersPresent.Add(c.Id);
        }

        public bool IsCentralCase()
        {
            return (Location.XPosition + Location.YPosition) % 9 != 0;
        }

        public List<ulong> GetCharactersOnCase()
        {
            return CharactersPresent;
        }

        public void RemoveCharacter(Character c)
        {
            if (CharactersPresent == null)
            {
                return;
            }

            CharactersPresent.Remove(c.Id);
        }
    }


}