using System.Collections.Generic;
using System.Xml;
using The_Storyteller.Models.MGameObject;
using The_Storyteller.Models.MGameObject.Resources;

namespace The_Storyteller.Models.MMap.MCase
{
    static class CaseFactory
    {
        public static Case BuildCase(string type, Location loc)
        {
            Case c;
            switch (type)
            {
                case "water": c = new WaterCase(); break;
                case "land": c = new LandCase(); break;
                case "desert": c = new DesertCase(); break;
                case "forest": c = new ForestCase(); break;
                case "village": c = new VillageCase(); break;
                case "mountain": c = new MountainCase(); break;
                default: c = new LandCase(); break;
            }

            c.Location = loc;
            c.IsAvailable = true;
            c.VillageId = ulong.MinValue;

            return c;
        }

        public static Case BuildCase(XmlElement caseXml)
        {
            Case c;

            //Type de case
            string type = caseXml.GetAttribute("type");
            switch (type.ToLower())
            {
                case "water": c = new WaterCase(); break;
                case "land": c = new LandCase(); break;
                case "desert": c = new DesertCase(); break;
                case "forest": c = new ForestCase(); break;
                case "village": c = new VillageCase(); break;
                case "mountain": c = new MountainCase(); break;
                default: c = new LandCase(); break;
            }

            //Location
            if (!int.TryParse(caseXml.GetAttribute("locationX"), out int posX))
            {
                posX = int.MinValue;
            }
            if (!int.TryParse(caseXml.GetAttribute("locationY"), out int posY))
            {
                posY = int.MinValue;
            }
            c.Location = new Location(posX, posY);
            
            //IsAvailable
            if (caseXml.GetAttribute("isAvailable").ToLower().Equals("true"))
            {
                c.IsAvailable = true;
            }
            else
            {
                c.IsAvailable = false;
            }

            //Villageid
            if (!ulong.TryParse(caseXml.GetAttribute("villageId"), out ulong villageId))
            {
                villageId = ulong.MinValue;
            }
            c.VillageId = villageId;

            List<Resource> resList = new List<Resource>();
            //Resources
            foreach(XmlElement resXml in caseXml.GetElementsByTagName("resources")[0].ChildNodes)
            {
                GameObject r = GameObjectFactory.BuildGameObject(resXml);
                resList.Add((Resource)r);
            }
            c.Resources = resList;

            List<ulong> charList = new List<ulong>();
            //characters
            foreach (XmlElement resXml in caseXml.GetElementsByTagName("characters")[0].ChildNodes)
            {
                if (ulong.TryParse(resXml.Value, out ulong charId))
                {
                    charList.Add(charId);
                }
               
            }
            c.CharactersPresent = charList;
            
            return c;
        }
    }
}
