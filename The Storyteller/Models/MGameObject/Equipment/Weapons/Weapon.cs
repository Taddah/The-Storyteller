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

        public override XmlElement Serialize(XmlDocument doc)
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

        public new static GameObject Build(XmlElement element)
        {
            if (!int.TryParse(element.GetAttribute("quantity"), out int quantity)) quantity = 0;
            if (!int.TryParse(element.GetAttribute("attackDamage"), out int attackDamage)) attackDamage = 0;
            if (!int.TryParse(element.GetAttribute("craftsmanId"), out int craftsmanId)) craftsmanId = 0;
            if (!int.TryParse(element.GetAttribute("hand"), out int hand)) hand = 0;

            return new Weapon
            {
                Name = element.GetAttribute("name"),
                Quantity = quantity,
                AttackDamage = attackDamage,
                CraftsmanId = craftsmanId,
                Hand = hand
            };
        }
    }
}
