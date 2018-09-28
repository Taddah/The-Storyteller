using Newtonsoft.Json;
using System.Collections.Generic;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MMap;

namespace The_Storyteller.Models.MVillage
{
    public enum VillagePermission
    {
        Free,
        villagers,
        Custom
    }

    class Village
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string RegionName { get; set; }
        public ulong KingId { get; set; }
        public VillagePermission VillagePermission { get; set; }
        public ulong InventoryId { get; set; }
        public List<ulong> WaitingList { get; set; }
        
        public List<Building> Buildings { get; set; }
        
        public List<ulong> Villagers { get; set; }

        public Village()
        {
            Buildings = new List<Building>();
            Villagers = new List<ulong>();
            WaitingList = new List<ulong>();
        }

        public void AddBuilding(Building b)
        {
            Buildings.Add(b);
        }

        public void AddInhabitant(Character c)
        {
            c.VillageName = Name;
            c.Profession = Profession.Villager;
            Villagers.Add(c.Id);
        }

        public void RemoveVillager(Character c)
        {
            c.VillageName = null;
            c.Profession = Profession.Peasant;
            Villagers.Remove(c.Id);
        }

        public List<ulong> GetInhabitants()
        {
            return this.Villagers;
        }

        public List<Building> GetBuildings()
        {
            return this.Buildings;
        }
    }
}