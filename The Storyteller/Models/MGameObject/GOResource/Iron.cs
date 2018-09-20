using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace The_Storyteller.Models.MGameObject.GOResource
{
    public class Iron : Ore
    {

        public Iron(int quantity = 0)
        {
            Name = "Iron";
            Quantity = quantity;
        }

        public override new static GameObject Build(XmlElement element)
        {
            if (!int.TryParse(element.GetAttribute("quantity"), out int quantity)) quantity = 0;

                return new Wood
                {
                    Name = element.GetAttribute("name"),
                    Quantity = quantity
                };
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement element = doc.CreateElement("object");
            element.SetAttribute("type", "iron");
            element.SetAttribute("name", this.Name);
            element.SetAttribute("quantity", this.Quantity.ToString());

            return element;
        }
    }
}