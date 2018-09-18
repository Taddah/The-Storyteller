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
