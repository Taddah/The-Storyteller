using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using The_Storyteller.Models.MMap;
using CaseType = The_Storyteller.Models.MMap.CaseType;

namespace The_Storyteller.Entities.Game
{
    internal class MapManager
    {
        private readonly string _filename;
        private readonly List<Region> _regions;

        public MapManager(string filename)
        {
            _filename = filename;
            _regions = LoadFromFile();
        }

        private List<Region> LoadFromFile()
        {
            if (!File.Exists(_filename))
                return new List<Region>();
            using (var sr = new StreamReader(_filename))
            {
                var res = JsonConvert.DeserializeObject<List<Region>>(sr.ReadToEnd());
                if (res != null) return res;
                return new List<Region>();
            }
        }

        private void SaveToFile()
        {
            using (var sw = new StreamWriter(_filename))
            {
                sw.Write(JsonConvert.SerializeObject(_regions));
            }
        }

        public Region GetAvailableRegion()
        {
            var sortedRegions = _regions.OrderBy(a => Guid.NewGuid()).ToList();
            foreach (Region r in sortedRegions)
            {
                if (r.GetCentralCase().IsAvailable && r.GetCentralCase().IsBuildable())
                    return r;
            }

            return null;
        }

        public RegionType GetRandomRegionType()
        {
            var random = new Random();
            var valuesRegionType = Enum.GetValues(typeof(RegionType));
            return (RegionType)valuesRegionType.GetValue(random.Next(valuesRegionType.Length));
        }

        public Region GenerateNewRegion(int size, ulong id, string name, RegionType regionType, bool forceValable = false)
        {
            var random = new Random();
            var r = new Region
            {
                Name = name,
                Id = id,
                Type = regionType
            };

            var values = Enum.GetValues(typeof(CaseType));

            var centralCase = GetNextCentralCase();
            var baseX = centralCase.XPosition - (int)Math.Floor(Convert.ToDecimal(size / 2));
            var endX = centralCase.XPosition + (int)Math.Floor(Convert.ToDecimal(size / 2));
            var baseY = centralCase.YPosition - (int)Math.Floor(Convert.ToDecimal(size / 2));
            var endY = centralCase.YPosition + (int)Math.Floor(Convert.ToDecimal(size / 2));

            for (var i = baseX; i <= endX; i++)
            {
                for (var j = baseY; j <= endY; j++)
                {
                    var c = new Case
                    {
                        Location = new Location(i, j),
                        IsAvailable = true
                    };

                    if (r.Type == RegionType.Sea) c.Type = CaseType.Water;
                    else if (r.Type == RegionType.Desert) c.Type = CaseType.Desert;
                    else if(r.Type == RegionType.Mountain)
                    {
                        var rndValue = random.Next(1, 10);
                        if(rndValue < 2) c.Type = CaseType.Water;
                        else if(rndValue < 4) c.Type = CaseType.Land;
                        else if (rndValue < 6) c.Type = CaseType.Forest;
                        else c.Type = CaseType.Mountain;
                    }
                    else
                    {
                        var rndValue = random.Next(1, 10);
                        if (rndValue < 3) c.Type = CaseType.Water;
                        else if (rndValue < 6) c.Type = CaseType.Forest;
                        else c.Type = CaseType.Land;
                    }
                    r.AddCase(c);
                }
            }

            //Force this region to have a valid central case
            if (forceValable)
                r.GetCentralCase().Type = CaseType.Land;

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
            foreach(Region r in _regions)
            {
                if (r.GetCase(l) != null) return r;
            }
            return null;
        }

        public Case GetCase(Location l)
        {
            foreach (Region r in _regions)
            {
                if (r.GetCase(l) != null) return r.GetCase(l);
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

        public Location GetNextCentralCase()
        {
            var locationFound = false;
            var nextLocation = new Location(0, 0);
            var rand = new Random();

            if (_regions.Count == 0) return nextLocation;

            do
            {
                var region = _regions[rand.Next(0, _regions.Count - 1)];
                var locRegion = region.GetCentralCase().Location;

                //Choose one direction randomly
                switch(rand.Next(0, 4))
                {
                    //South
                    case 0: nextLocation = new Location()
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

                if (!RegionExist(nextLocation)) locationFound = true;
            } while (!locationFound);

            return nextLocation;
        }
    }
}