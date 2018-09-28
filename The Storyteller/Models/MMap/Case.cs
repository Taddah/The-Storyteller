using System.Collections.Generic;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MGameObject.Resources;
using The_Storyteller.Models.MVillage;

namespace The_Storyteller.Models.MMap
{

    internal abstract class Case
    {
        public Location Location { get; set; }
        public bool IsAvailable { get; set; }
        public ulong VillageId { get; set; }
        public List<Resource> Resources { get; set; }

        //Character présent sur la map
        public List<ulong> CharacterPresent { get; set; }

        public Case()
        {
            CharacterPresent = new List<ulong>();
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

        public void AddNewCharacter(Character c)
        {
            if (CharacterPresent == null) return;
            CharacterPresent.Add(c.Id);
        }

        public bool IsCentralCase()
        {
            return (Location.XPosition + Location.YPosition) % 9 != 0;
        }

        public List<ulong> GetCharactersOnCase()
        {
            return this.CharacterPresent;
        }

        public void RemoveCharacter(Character c)
        {
            if (CharacterPresent == null) return;
            CharacterPresent.Remove(c.Id);
        }
    }

   
}