using Newtonsoft.Json;
using System.Collections.Generic;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MMap;

namespace The_Storyteller.Models.MVillage
{
    class Village
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RegionName { get; set; }
        public ulong MayorId { get; set; }
        public VillageInventory Inventory { get; set; }

        [JsonProperty("buildings")]
        private List<Building> _buildings { get; set; }

        [JsonProperty("inhabitantId")]
        private List<int> _inhabitantId { get; set; }

        public Village()
        {
            _buildings = new List<Building>();
            _inhabitantId = new List<int>();
        }

        public void AddBuilding(Building b)
        {
            _buildings.Add(b);
        }

        public void AddInhabitant(Character c)
        {
            _inhabitantId.Add(c.Id);
        }
    }
}