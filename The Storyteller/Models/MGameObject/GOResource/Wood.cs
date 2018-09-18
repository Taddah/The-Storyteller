using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace The_Storyteller.Models.MGameObject.GOResource
{
    public class Wood : Resource
    {

        public Wood(int quantity = 0)
        {
            Name = "Wood";
            Quantity = quantity;
        }
        

        public override XmlElement Seralize(XmlDocument doc)
        {
            XmlElement element = doc.CreateElement("object");
            element.SetAttribute("type", "wood");
            element.SetAttribute("name", this.Name);
            element.SetAttribute("quantity", this.Quantity.ToString());

            return element;
        }
    }
}