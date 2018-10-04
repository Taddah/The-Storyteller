using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using The_Storyteller.Models.MMap;
using The_Storyteller.Models.MMap.MCase;

namespace The_Storyteller.Entities.Game
{
    public enum Direction { North, East, West, South, Unknown };

    /// <summary>
    /// Gère la map en général (sauvegardé dans un json)
    /// La map est composé de région
    /// Chaque région est composé de 9x9 cases
    /// Une région peut-être habitable ou non
    /// Cette classe permet de générer une nouvelle région
    /// </summary>
    internal class MapManager
    {
        private readonly string _filename;
        private readonly List<Region> _regions;

        public MapManager(string filename)
        {
            _filename = filename;
            _regions = LoadFromFile();

            Task task = new Task(() => DoPeriodicCharacterSave());
            task.Start();
        }

        private void DoPeriodicCharacterSave()
        {
            while (true)
            {
                try
                {
                    SaveToFile();
                }
                catch (IOException)
                {

                }

                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
        }

        private List<Region> LoadFromFile()
        {
            if (!File.Exists(_filename))
            {
                return new List<Region>();
            }

            List<Region> listReg = new List<Region>();

            XmlDocument doc = new XmlDocument();
            doc.Load(_filename);

            //get inventories
            XmlNode regions = doc.GetElementsByTagName("regions").Item(0);

            //Pour chaque inventaire
            foreach (XmlElement region in regions.ChildNodes)
            {
                if (!ulong.TryParse(region.GetAttribute("id"), out ulong id))
                {
                    break;
                }

                Region reg = new Region
                {
                    Id = id,
                    Name = region.GetAttribute("name"),
                    Type = GetTypeFromString(region.GetAttribute("type"))
                };

                //Pour chaque case
                foreach (XmlElement caseXml in region.ChildNodes)
                {
                    Case c = CaseFactory.BuildCase(caseXml);
                    reg.AddCase(c);
                }
                listReg.Add(reg);
            }

            return listReg;
        }

        private RegionType GetTypeFromString(string type)
        {
            switch (type.ToLower())
            {
                case "plain": return RegionType.Plain;
                case "desert": return RegionType.Desert;
                case "mountain": return RegionType.Mountain;
                case "sea": return RegionType.Sea;
                default: return RegionType.Plain;
            }
        }

        private void SaveToFile()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("regions");

            foreach (Region r in _regions)
            {
                XmlElement regionXml = doc.CreateElement("region");
                regionXml.SetAttribute("id", r.Id.ToString());
                regionXml.SetAttribute("name", r.Name);
                regionXml.SetAttribute("type", r.Type.ToString());

                foreach (Case c in r.GetAllCases())
                {
                    XmlElement xmlGo = c.Serialize(doc);
                    regionXml.AppendChild(xmlGo);
                }

                root.AppendChild(regionXml);
            }

            doc.AppendChild(root);

            using (StreamWriter sw = new StreamWriter(_filename))
            {
                doc.Save(sw);
            }
        }

        public Region GetAvailableRegion()
        {
            List<Region> sortedRegions = _regions.OrderBy(a => Guid.NewGuid()).ToList();
            foreach (Region r in sortedRegions)
            {
                if (r.GetCentralCase().IsAvailable && r.GetCentralCase().IsBuildable() && r.Id == 0)
                {
                    return r;
                }
            }

            return null;
        }

        public RegionType GetRandomRegionType()
        {
            Random random = new Random();
            Array valuesRegionType = Enum.GetValues(typeof(RegionType));
            return (RegionType)valuesRegionType.GetValue(random.Next(valuesRegionType.Length));
        }

        public Region GenerateNewRegion(int size, ulong id, string name, RegionType regionType, Location centralCase = null, bool forceValable = false)
        {
            Random random = new Random();
            Region r = new Region
            {
                Name = name,
                Id = id,
                Type = regionType
            };


            if (centralCase == null)
            {
                centralCase = GetRandomNextCentralCase();
            }

            int baseX = centralCase.XPosition - (int)Math.Floor(Convert.ToDecimal(size / 2));
            int endX = centralCase.XPosition + (int)Math.Floor(Convert.ToDecimal(size / 2));
            int baseY = centralCase.YPosition - (int)Math.Floor(Convert.ToDecimal(size / 2));
            int endY = centralCase.YPosition + (int)Math.Floor(Convert.ToDecimal(size / 2));

            for (int i = baseX; i <= endX; i++)
            {
                for (int j = baseY; j <= endY; j++)
                {
                    Case c;

                    if (r.Type == RegionType.Sea)
                    {
                        c = CaseFactory.BuildCase("water", new Location(i, j));
                    }
                    else if (r.Type == RegionType.Desert)
                    {
                        c = CaseFactory.BuildCase("desert", new Location(i, j));
                    }
                    else if (r.Type == RegionType.Mountain)
                    {
                        int rndValue = random.Next(1, 10);
                        if (rndValue < 2)
                        {
                            c = CaseFactory.BuildCase("water", new Location(i, j));
                        }
                        else if (rndValue < 4)
                        {
                            c = CaseFactory.BuildCase("land", new Location(i, j));
                        }
                        else if (rndValue < 6)
                        {
                            c = CaseFactory.BuildCase("forest", new Location(i, j));
                        }
                        else
                        {
                            c = CaseFactory.BuildCase("mountain", new Location(i, j));
                        }
                    }
                    else
                    {
                        int rndValue = random.Next(1, 10);
                        if (rndValue < 3)
                        {
                            c = CaseFactory.BuildCase("water", new Location(i, j));
                        }
                        else if (rndValue < 6)
                        {
                            c = CaseFactory.BuildCase("forest", new Location(i, j));
                        }
                        else
                        {
                            c = CaseFactory.BuildCase("land", new Location(i, j));
                        }
                    }
                    r.AddCase(c);
                }
            }

            //Force this region to have a valid central case
            if (forceValable)
            {
                r.SetCentralCase(CaseFactory.BuildCase("land", r.GetCentralCase().Location));
            }

            _regions.Add(r);
            SaveToFile();

            return r;
        }

        public Region GetRegionByName(string name)
        {
            return _regions.SingleOrDefault(reg => reg.Name.ToLower() == name.ToLower());
        }

        public Region GetRegionByLocation(Location l)
        {
            foreach (Region r in _regions)
            {
                if (r.GetCase(l) != null)
                {
                    return r;
                }
            }
            return null;
        }

        public Case GetCase(Location l)
        {
            foreach (Region r in _regions)
            {
                if (r.GetCase(l) != null)
                {
                    return r.GetCase(l);
                }
            }
            return null;
        }

        public bool RegionExist(Location l)
        {
            return _regions.Exists(reg => reg.GetCentralCase().Location.Equals(l));
        }

        public bool IsRegionNameTaken(string name)
        {
            return _regions.Exists(reg => reg.Name.ToLower() == name.ToLower());
        }

        /// <summary>
        /// Récupère la case central à côté de celle fourni en paramètre en fonction de 
        /// la direction demandé"
        /// </summary>
        /// <param name="centralCase"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Location GetCentralCaseByDirection(Case centralCase, Direction direction)
        {
            if (_regions == null || _regions.Count == 0)
            {
                return null;
            }

            switch (direction)
            {
                case Direction.North:
                    return new Location()
                    {
                        XPosition = centralCase.Location.XPosition,
                        YPosition = centralCase.Location.YPosition + _regions[0].GetSize()
                    };
                case Direction.South:
                    return new Location()
                    {
                        XPosition = centralCase.Location.XPosition,
                        YPosition = centralCase.Location.YPosition - _regions[0].GetSize()
                    };
                case Direction.East:
                    return new Location()
                    {
                        XPosition = centralCase.Location.XPosition + _regions[0].GetSize(),
                        YPosition = centralCase.Location.YPosition
                    };
                case Direction.West:
                    return new Location()
                    {
                        XPosition = centralCase.Location.XPosition - _regions[0].GetSize(),
                        YPosition = centralCase.Location.YPosition
                    };
                default: return null;
            }
        }

        /// <summary>
        /// Retourne une case centrale au hasard pour générer une nouvelle région
        /// à partir de cette case
        /// Cette case central sera situé à côté d'une déjà existante
        /// </summary>
        /// <returns></returns>
        public Location GetRandomNextCentralCase()
        {
            bool locationFound = false;
            Location nextLocation = new Location(0, 0);
            Random rand = new Random();

            if (_regions.Count == 0)
            {
                return nextLocation;
            }

            do
            {
                Region region = _regions[rand.Next(0, _regions.Count - 1)];
                Location locRegion = region.GetCentralCase().Location;

                //Choix d'une direction aléatoire
                switch (rand.Next(0, 4))
                {
                    //South
                    case 0:
                        nextLocation = new Location()
                        {
                            XPosition = locRegion.XPosition,
                            YPosition = locRegion.YPosition - region.GetSize()
                        }; break;
                    //North
                    case 1:
                        nextLocation = new Location()
                        {
                            XPosition = locRegion.XPosition,
                            YPosition = locRegion.YPosition + region.GetSize()
                        }; break;
                    //East
                    case 2:
                        nextLocation = new Location()
                        {
                            XPosition = locRegion.XPosition + region.GetSize(),
                            YPosition = locRegion.YPosition
                        }; break;
                    //West
                    case 3:
                        nextLocation = new Location()
                        {
                            XPosition = locRegion.XPosition - region.GetSize(),
                            YPosition = locRegion.YPosition
                        }; break;
                    default:
                        nextLocation = new Location()
                        {
                            XPosition = locRegion.XPosition,
                            YPosition = locRegion.YPosition - region.GetSize()
                        }; break;
                }

                if (!RegionExist(nextLocation))
                {
                    locationFound = true;
                }
            } while (!locationFound);

            return nextLocation;
        }
    }
}