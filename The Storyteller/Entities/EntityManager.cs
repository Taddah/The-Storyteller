using The_Storyteller.Entities.Game;

namespace The_Storyteller.Entities
{
    /// <summary>
    /// Strategy
    /// Regroupe les entités du jeu (Character, Guild, Map, Village, ...)
    /// </summary>
    internal class EntityManager
    {
        public EntityManager()
        {
            Guilds = new GuildManager("guilds.json");
            Characters = new CharacterManager("characters.json");
            Map = new MapManager("map.xml");
            Villages = new VillageManager("villages.json");
            Inventories = new InventoryManager("inventories.xml");
        }

        public CharacterManager Characters { get; set; }
        public GuildManager Guilds { get; set; }
        public MapManager Map { get; set; }
        public VillageManager Villages { get; set; }
        public InventoryManager Inventories { get; set; }
    }
}