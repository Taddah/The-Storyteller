using System.Collections.Generic;
using The_Storyteller.Models.MCharacter;

namespace The_Storyteller.Models.MVillage
{
    class VillageInventory
    {
        public int Money { get; set; }
        public List<GameObject> GameObjects { get; set; }
    }
}