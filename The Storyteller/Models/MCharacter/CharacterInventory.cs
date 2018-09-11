using System.Collections.Generic;

namespace The_Storyteller.Models.MCharacter
{
    class CharacterInventory
    {
        public int Money { get; set; }
        public List<GameObject> GameObjects { get; set; }

        public CharacterInventory()
        {
            GameObjects = new List<GameObject>();
        }

    }
}