using System.Collections.Generic;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MMap;

namespace The_Storyteller.Models.MVillage
{
    class Village
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Region Region { get; set; }
        public Character Mayor { get; set; }
        public VillageInventory Inventory { get; set; }
        public List<Building> Buildings { get; set; }
        public List<int> InhabitantId { get; set; }
    }
}