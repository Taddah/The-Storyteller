using System.Xml;

namespace The_Storyteller.Models.MGameObject.Resources
{
    public class Wheat : Resource
    {

        public Wheat(int quantity = 0)
        {
            Name = "Wheat";
            Quantity = quantity;
        }

        public new static GameObject Build(XmlElement element)
        {
            if (!int.TryParse(element.GetAttribute("quantity"), out int quantity)) quantity = 0;

                return new Wheat
                {
                    Name = element.GetAttribute("name"),
                    Quantity = quantity
                };
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement element = doc.CreateElement("object");
            element.SetAttribute("type", "wheat");
            element.SetAttribute("name", this.Name);
            element.SetAttribute("quantity", this.Quantity.ToString());

            return element;
        }
    }
}