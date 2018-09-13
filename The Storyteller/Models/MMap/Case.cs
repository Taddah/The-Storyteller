﻿using System.Collections.Generic;
using The_Storyteller.Models.MCharacter;
using The_Storyteller.Models.MVillage;

namespace The_Storyteller.Models.MMap
{
    public enum CaseType
    {
        Water,
        Forest,
        Desert,
        Mountain,
        Land
    }

    internal class Case
    {
        public Location Location { get; set; }
        public CaseType Type { get; set; }
        public bool IsAvailable { get; set; }
        public int VillageId { get; set; }

        //Character présent sur la map
        public List<Character> CharacterPresent { get; set; }

        public Case()
        {
            CharacterPresent = new List<Character>();
        }

        /// <summary>
        /// Case constructible pour un village
        /// </summary>
        /// <returns></returns>
        public bool IsBuildable()
        {
            if (Type == CaseType.Mountain
                || Type == CaseType.Water
                || Type == CaseType.Desert) return false;

            return true;
        }

        public void AddNewCharacter(Character c)
        {
            if (CharacterPresent == null) return;
            CharacterPresent.Add(c);
        }

        public List<Character> GetCharactersOnCase()
        {
            return this.CharacterPresent;
        }

        public void RemoveCharacter(Character c)
        {
            if (CharacterPresent == null) return;
            CharacterPresent.Remove(c);
        }
    }

   
}