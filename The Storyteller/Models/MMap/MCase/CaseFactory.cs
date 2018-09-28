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
    }
}
