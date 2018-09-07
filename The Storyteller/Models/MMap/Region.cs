using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace The_Storyteller.Models.MMap
{
    public enum RegionType { Plain, Desert, Mountain, Sea}
    internal class Region
    {
        [JsonProperty("discordID")]
        public ulong Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public RegionType Type { get; set; }

        [JsonProperty("cases")]
        private List<Case> _cases;

        public Region()
        {
            _cases = new List<Case>();
        }

        public void AddCase(Case c)
        {
            _cases.Add(c);
        }

        public Case GetCase(Location l)
        {
            return _cases.SingleOrDefault(c =>
                c.Location.XPosition == l.XPosition && c.Location.YPosition == l.YPosition);
        }

        public int GetSize()
        {
            var size = Math.Sqrt(_cases.Count());
            return Convert.ToInt16(size);
        }

        public List<Case> GetAllCases()
        {
            return _cases;
        }

        public Case GetCentralCase()
        {
            if(_cases.Count > 0 && _cases.Count == GetSize()*GetSize())
                return _cases[(int) Math.Floor((decimal) (GetSize() * GetSize()) / 2)];

            return null;
        }
    }
}