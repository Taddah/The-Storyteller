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

        [JsonProperty("buildings")]
        private List<Building> _buildings { get; set; }

        [JsonProperty("inhabitantId")]
        private List<ulong> _inhabitantId { get; set; }

        public Village()
        {
            _buildings = new List<Building>();
            _inhabitantId = new List<ulong>();
            WaitingList = new List<ulong>();
        }

        public void AddBuilding(Building b)
        {
            _buildings.Add(b);
        }

        public void AddInhabitant(Character c)
        {
            c.VillageName = Name;
            _inhabitantId.Add(c.Id);
        }

        public List<ulong> GetInhabitants()
        {
            return this._inhabitantId;
        }

        public List<Building> GetBuildings()
        {
            return this._buildings;
        }
    }
}