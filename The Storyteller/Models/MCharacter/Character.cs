using System.Xml.Serialization;
using The_Storyteller.Models.MGameObject.Equipment.Weapons;
using The_Storyteller.Models.MGameObject.GOResource;
using The_Storyteller.Models.MMap;
using The_Storyteller.Models.MVillage;

namespace The_Storyteller.Models.MCharacter
{
    public enum Sex
    {
        Male,
        Female
    }

    public enum Profession
    {
        Peasant,
        Villager,
        King
    }

    public class Character
    {
        public int Id { get; set; }
        public ulong DiscordID { get; set; }
        public string Name { get; set; }
        public string TrueName { get; set; }
        public Sex Sex { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Energy { get; set; }
        public int MaxEnergy { get; set; }
        public Location Location { get; set; }
        public CharacterStats Stats { get; set; }
        public string OriginRegionName { get; set; }
        public string VillageName { get; set; }
        public Profession Profession { get; set; }
    }
}