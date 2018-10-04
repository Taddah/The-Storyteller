using System.Xml;
using The_Storyteller.Models.MGameObject.Equipment.Weapons;
using The_Storyteller.Models.MGameObject.Others;
using The_Storyteller.Models.MGameObject.Resources;
using The_Storyteller.Models.MGameObject.Resources.Constructions;
using The_Storyteller.Models.MGameObject.Resources.Cookables;
using The_Storyteller.Models.MGameObject.Resources.Ore;

namespace The_Storyteller.Models.MGameObject
{
    public static class GameObjectFactory
    {
        public static GameObject BuildGameObject(XmlElement element)
        {
            string type = element.GetAttribute("type");
            switch (type)
            {
                case "money": return Money.Build(element);
                case "wood": return Wood.Build(element);
                case "weapon": return Weapon.Build(element);
                case "coal": return Coal.Build(element);
                case "copper": return Copper.Build(element);
                case "gold": return Gold.Build(element);
                case "iron": return Iron.Build(element);
                case "silver": return Silver.Build(element);
                case "leather": return Leather.Build(element);
                case "meat": return Meat.Build(element);
                case "sand": return Sand.Build(element);
                case "stone": return Stone.Build(element);
                case "water": return Water.Build(element);
                case "wheat": return Wheat.Build(element);
                default: return null;
            }
        }
    }
}
