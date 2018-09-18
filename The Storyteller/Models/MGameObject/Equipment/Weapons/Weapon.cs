using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace The_Storyteller.Models.MGameObject.Equipment.Weapons
{
    public class Weapon : GameObject
    {
        public int AttackDamage { get; set; }
        public int CraftsmanId { get; set; }
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

        public override XmlElement Seralize(XmlDocument doc)
        {
            XmlElement element = doc.CreateElement("object");
            element.SetAttribute("type", "weapon");
            element.SetAttribute("name", this.Name);
            element.SetAttribute("quantity", this.Quantity.ToString());
            element.SetAttribute("attackDamage", this.AttackDamage.ToString());
            element.SetAttribute("craftsmanId", this.CraftsmanId.ToString());
            element.SetAttribute("hand", this.Hand.ToString());

            return element;
        }
    }
}
