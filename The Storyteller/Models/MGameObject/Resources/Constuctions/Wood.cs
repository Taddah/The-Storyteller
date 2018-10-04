using System.Xml;
using The_Storyteller.Models.MCharacter.MCharacterSkills;

namespace The_Storyteller.Models.MGameObject.Resources.Constructions
{
    public class Wood : Resource
    {

        public Wood(int quantity = 0)
        {
            Name = "Wood";
            Quantity = quantity;
            AssociatedSkill = new LoggerSkill();
            HarvestableQuantity = 5;
        }

        public new static GameObject Build(XmlElement element)
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
            element.SetAttribute("type", "wood");
            element.SetAttribute("name", this.Name);
            element.SetAttribute("quantity", this.Quantity.ToString());

            return element;
        }
    }
}