using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace The_Storyteller.Models.MGameObject.Others
{
    class Money : GameObject
    {

        public Money(int quantity = 0)
        {
            Name = "Money";
            Quantity = quantity;
        }

        public new static GameObject Build(XmlElement element)
        {
            if (!int.TryParse(element.GetAttribute("quantity"), out int quantity)) quantity = 0;

            return new Money
            {
                Name = element.GetAttribute("name"),
                Quantity = quantity
            };
        }

        public override XmlElement Seralize(XmlDocument doc)
        {
            XmlElement element = doc.CreateElement("object");
            element.SetAttribute("type", "money");
            element.SetAttribute("name", this.Name);
            element.SetAttribute("quantity", this.Quantity.ToString());

            return element;
        }

    }
}
