using Newtonsoft.Json;
using System;
using System.Xml;
using System.Xml.Serialization;
using The_Storyteller.Models.MGameObject.Equipment.Weapons;
using The_Storyteller.Models.MGameObject.GOResource;

namespace The_Storyteller.Models.MGameObject
{
    public abstract class GameObject : ICloneable
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public abstract XmlElement Seralize(XmlDocument doc);
    }
}