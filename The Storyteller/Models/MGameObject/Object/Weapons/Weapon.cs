using System;
using System.Collections.Generic;
using System.Text;
namespace The_Storyteller.Models.MGameObject.Object.Weapons
{
    class Weapon : GameObject
    {
        public int AttackDamage { get; set; }
        public int CraftsmanId { get; set; }
        public int Hand { get; set; }

        public Weapon(string name, int value, int attackDamage, int craftsmanId, int handNumber)
        {
            Name = name;
            Value = value;
            AttackDamage = attackDamage;
            CraftsmanId = craftsmanId;
            Hand = handNumber;
        }


    }
}
