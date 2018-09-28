namespace The_Storyteller.Models.MMap.MCase
{
    class VillageCase : Case
    {
        public override void GenerateResources() { }

        public override bool IsBuildable()
        {
            return false;
        }

        public override string GetTypeOfCase()
        {
            return "Village";
        }
    }
}
