using System;
using System.Xml;

namespace The_Storyteller.Models.MCharacter
{
    public class CharacterStats
    {
        public int Health {get; set;}
        public int MaxHealth { get; set; }
        public int Endurance { get; set; }
        public int Strength { get; set; }
        public int Intelligence { get; set; }
        public int Agility { get; set; }
        public int Dexterity { get; set; }
        public int UpgradePoint { get; set; }

        public XmlElement Serialize(XmlDocument doc)
        {
            XmlElement xmlStats = doc.CreateElement("stats");
            xmlStats.SetAttribute("health", Health.ToString());
            xmlStats.SetAttribute("maxHealth", MaxHealth.ToString());
            xmlStats.SetAttribute("endurance", Endurance.ToString());
            xmlStats.SetAttribute("strength", Strength.ToString());
            xmlStats.SetAttribute("intelligence", Intelligence.ToString());
            xmlStats.SetAttribute("agility", Agility.ToString());
            xmlStats.SetAttribute("dexterity", Dexterity.ToString());
            xmlStats.SetAttribute("upgradePoint", UpgradePoint.ToString());

            return xmlStats;
        }

        internal static CharacterStats Deserialize(XmlElement xml)
        {
            int.TryParse(xml.GetAttribute("health"), out int health);
            int.TryParse(xml.GetAttribute("maxHealth"), out int maxHealth);
            int.TryParse(xml.GetAttribute("endurance"), out int endurance);
            int.TryParse(xml.GetAttribute("strength"), out int strength);
            int.TryParse(xml.GetAttribute("intelligence"), out int intelligence);
            int.TryParse(xml.GetAttribute("agility"), out int agility);
            int.TryParse(xml.GetAttribute("dexterity"), out int dexterity);
            int.TryParse(xml.GetAttribute("upgradePoint"), out int upgradePoint);

            return new CharacterStats()
            {
                Health = health,
                Agility = agility,
                Dexterity = dexterity,
                Endurance = endurance,
                Intelligence = intelligence,
                MaxHealth = maxHealth,
                Strength = strength,
                UpgradePoint = upgradePoint
            };
        }
    }
}