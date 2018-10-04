using System.Xml;

namespace The_Storyteller.Models.MGameObject.Resources.Constructions
{
    public class Sand : Resource
    {

        public Sand(int quantity = 0)
        {
            Name = "Sand";
            Quantity = quantity;
        }

        public new static GameObject Build(XmlElement element)
        {
            if (!int.TryParse(element.GetAttribute("quantity"), out int quantity)) quantity = 0;

                return new Sand
                {
                    Name = element.GetAttribute("name"),
                    Quantity = quantity
                };
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement element = doc.CreateElement("object");
            element.SetAttribute("type", "sand");
            element.SetAttribute("name", this.Name);
            element.SetAttribute("quantity", this.Quantity.ToString());

            return element;
        }
    }
}