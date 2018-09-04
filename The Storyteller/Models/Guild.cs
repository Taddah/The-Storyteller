using System;
using The_Storyteller.Models.MMap;

namespace The_Storyteller.Models
{
    [Serializable]
    internal class Guild
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public int MemberCount { get; set; }
        public Region Region { get; set; }
        public Location SpawnLocation { get; set; }
    }
}