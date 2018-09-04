using The_Storyteller.Entities.Game;

namespace The_Storyteller.Entities
{
    internal class EntityManager
    {
        public EntityManager()
        {
            Guilds = new GuildManager("guilds.json");
            Characters = new CharacterManager("characters.json");
            Map = new MapManager("map.json");
        }

        public CharacterManager Characters { get; set; }
        public GuildManager Guilds { get; set; }
        public MapManager Map { get; set; }
    }
}