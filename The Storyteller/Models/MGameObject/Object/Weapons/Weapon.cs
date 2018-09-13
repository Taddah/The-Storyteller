using System;
using System.Collections.Generic;
using System.Text;
using The_Storyteller.Models.MCharacter;

namespace The_Storyteller.Models.MGameObject.Object.Weapons
{
    class Weapon : GameObject
    {
        public int AttackDamage { get; set; }
        public Character Craftsman { get; set; }
        public int Hand { get; set; }

        public Weapon(string name, int value, int attackDamage, Character craftsman, int handNumber)
        {
            Name = name;
            Value = value;
            AttackDamage = attackDamage;
            Craftsman = craftsman;
            Hand = handNumber;
        }


    }
}
