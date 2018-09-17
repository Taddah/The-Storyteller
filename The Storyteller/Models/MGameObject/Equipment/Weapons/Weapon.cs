using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace The_Storyteller.Models.MGameObject.Equipment.Weapons
{
    public class Weapon : GameObject
    {
        [JsonProperty("AttackDamage")]
        public int AttackDamage { get; set; }
        [JsonProperty("CraftsmanId")]
        public int CraftsmanId { get; set; }
        [JsonProperty("Hand")]
        public int Hand { get; set; }

        public Weapon(string name, int attackDamage, int craftsmanId, int handNumber, int quantity = 1)
        {
            Name = name;
            Quantity = quantity;
            AttackDamage = attackDamage;
            CraftsmanId = craftsmanId;
            Hand = handNumber;
        }

        public Weapon() { }
    }
}
