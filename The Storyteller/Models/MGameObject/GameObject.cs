using System;
using System.Xml;

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

        public abstract XmlElement Serialize(XmlDocument doc);

        public static GameObject Build(XmlElement element)
        {
            throw new Exception("Build function is not implemented");
        }
    }
}