using System;
using System.Collections.Generic;
using System.Xml;
using The_Storyteller.Models.MCharacter.MCharacterSkills;

namespace The_Storyteller.Models.MCharacter
{
    public abstract class CharacterSkills
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }

        public int GetExperienceForNextLevel()
        {
            return Level * (Level - 1) * 2 + 5;
        }

        internal static List<CharacterSkills> Deserialize(XmlElement xml)
        {
            List<CharacterSkills> listSkills = new List<CharacterSkills>();

            foreach(XmlElement element in xml.ChildNodes)
            {
                CharacterSkills skill;

                switch (element.GetAttribute("name").ToLower())
                {
                    case "logger": skill = new LoggerSkill(); break;
                    default: return null;
                }

                int.TryParse(element.GetAttribute("level"), out int level);
                int.TryParse(element.GetAttribute("experience"), out int experience);

                skill.Level = level;
                skill.Experience = experience;

                listSkills.Add(skill);
            }
            

            return listSkills;
        }
    }
}
