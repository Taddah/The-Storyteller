using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace The_Storyteller.Models.MGameObject.GOResource
{
    [JsonObject("wood")]
    public class Wood : Resource
    {

        public Wood(int quantity = 0)
        {
            Name = "Wood";
            Quantity = quantity;
        }

        public Wood() { }
    }
}