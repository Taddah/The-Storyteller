using System.Xml;

namespace The_Storyteller.Models.MGameObject.Resources
{
    class Water : Resource
    {
        public Water(int quantity = 0)
        {
            Name = "Water";
            Quantity = quantity;
        }

        public new static GameObject Build(XmlElement element)
        {
            if (!int.TryParse(element.GetAttribute("quantity"), out int quantity)) quantity = 0;

            return new Water
            {
                Name = element.GetAttribute("name"),
                Quantity = quantity
            };
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement element = doc.CreateElement("object");
            element.SetAttribute("type", "water");
            element.SetAttribute("name", this.Name);
            element.SetAttribute("quantity", this.Quantity.ToString());

            return element;
        }
    }
}
