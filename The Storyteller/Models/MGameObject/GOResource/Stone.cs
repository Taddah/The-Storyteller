using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace The_Storyteller.Models.MGameObject.GOResource
{
    public class Stone : Resource
    {

        public Stone(int quantity = 0)
        {
            Name = "Stone";
            Quantity = quantity;
        }

        public new static GameObject Build(XmlElement element)
        {
            if (!int.TryParse(element.GetAttribute("quantity"), out int quantity)) quantity = 0;

                return new Stone
                {
                    Name = element.GetAttribute("name"),
                    Quantity = quantity
                };
        }

        public override XmlElement Seralize(XmlDocument doc)
        {
            XmlElement element = doc.CreateElement("object");
            element.SetAttribute("type", "Stone");
            element.SetAttribute("name", this.Name);
            element.SetAttribute("quantity", this.Quantity.ToString());

            return element;
        }
    }
}